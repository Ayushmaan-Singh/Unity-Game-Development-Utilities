using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace AstekUtility.ScriptableRenderPipelines
{
	public partial class RenderFinalEffect
	{

		private void Create_Outlines()
		{
			if (renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
				renderPassEvent = RenderPassEvent.BeforeRenderingPrePasses;

			viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(renderPassEvent, outlinesLayerMask, outlinesOccluderLayerMask, viewSpaceNormalsTextureSettings);
			screenSpaceOutlinePass = new ScreenSpaceOutlinePass(renderPassEvent, outlineSettings);
		}

		private void RenderPasses_Outlines(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			renderer.EnqueuePass(viewSpaceNormalsTexturePass);
			renderer.EnqueuePass(screenSpaceOutlinePass);
		}
		#region Variables

		[Space(20)]
		[Header("=====Outlines=====")]
		[Space(10)]
		[SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
		[SerializeField] private LayerMask outlinesLayerMask;
		[SerializeField] private LayerMask outlinesOccluderLayerMask;

		[SerializeField] private ScreenSpaceOutlineSettings outlineSettings = new ScreenSpaceOutlineSettings();
		[SerializeField] private ViewSpaceNormalsTextureSettings viewSpaceNormalsTextureSettings = new ViewSpaceNormalsTextureSettings();

		private ViewSpaceNormalsTexturePass viewSpaceNormalsTexturePass;
		private ScreenSpaceOutlinePass screenSpaceOutlinePass;

		#endregion

		#region Outline Render Classes

		[Serializable]
		private class ScreenSpaceOutlineSettings
		{

			[Header("General Outline Settings")]
			public Color outlineColor = Color.black;
			[Range(0.0f, 20.0f)]
			public float outlineScale = 1.0f;

			[Header("Depth Settings")]
			[Range(0.0f, 100.0f)]
			public float depthThreshold = 1.5f;
			[Range(0.0f, 500.0f)]
			public float robertsCrossMultiplier = 100.0f;

			[Header("Normal Settings")]
			[Range(0.0f, 1.0f)]
			public float normalThreshold = 0.4f;

			[Header("Depth Normal Relation Settings")]
			[Range(0.0f, 2.0f)]
			public float steepAngleThreshold = 0.2f;
			[Range(0.0f, 500.0f)]
			public float steepAngleMultiplier = 25.0f;
		}

		[Serializable]
		private class ViewSpaceNormalsTextureSettings
		{

			[Header("General Scene View Space Normal Texture Settings")]
			public RenderTextureFormat colorFormat;
			public int depthBufferBits = 16;
			public FilterMode filterMode;
			public Color backgroundColor = Color.black;

			[Header("View Space Normal Texture Object Draw Settings")]
			public PerObjectData perObjectData;
			public bool enableDynamicBatching;
			public bool enableInstancing;
		}

		private class ViewSpaceNormalsTexturePass : ScriptableRenderPass
		{

			private readonly RTHandle normals;
			private readonly Material normalsMaterial;
			private readonly Material occludersMaterial;

			private readonly List<ShaderTagId> shaderTagIdList;
			private FilteringSettings filteringSettings;

			private readonly ViewSpaceNormalsTextureSettings normalsTextureSettings;
			private FilteringSettings occluderFilteringSettings;

			public ViewSpaceNormalsTexturePass(RenderPassEvent renderPassEvent, LayerMask layerMask, LayerMask occluderLayerMask, ViewSpaceNormalsTextureSettings settings)
			{
				this.renderPassEvent = renderPassEvent;
				normalsTextureSettings = settings;
				filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
				occluderFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, occluderLayerMask);

				shaderTagIdList = new List<ShaderTagId>
				{
					new ShaderTagId("UniversalForward"),
					new ShaderTagId("UniversalForwardOnly"),
					new ShaderTagId("LightweightForward"),
					new ShaderTagId("SRPDefaultUnlit")
				};

				normals = RTHandles.Alloc("_SceneViewSpaceNormals", "_SceneViewSpaceNormals");
				normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));

				occludersMaterial = new Material(Shader.Find("Hidden/UnlitColor"));
				occludersMaterial.SetColor("_Color", normalsTextureSettings.backgroundColor);
			}

			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
				RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
				normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
				normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
				cmd.GetTemporaryRT(Shader.PropertyToID(normals.name), normalsTextureDescriptor, normalsTextureSettings.filterMode);

				ConfigureTarget(normals);
				ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (!normalsMaterial || !occludersMaterial)
					return;

				CommandBuffer cmd = CommandBufferPool.Get();
				using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation")))
				{
					context.ExecuteCommandBuffer(cmd);
					cmd.Clear();

					DrawingSettings drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
					drawSettings.perObjectData = normalsTextureSettings.perObjectData;
					drawSettings.enableDynamicBatching = normalsTextureSettings.enableDynamicBatching;
					drawSettings.enableInstancing = normalsTextureSettings.enableInstancing;
					drawSettings.overrideMaterial = normalsMaterial;

					DrawingSettings occluderSettings = drawSettings;
					occluderSettings.overrideMaterial = occludersMaterial;

					context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
					context.DrawRenderers(renderingData.cullResults, ref occluderSettings, ref occluderFilteringSettings);
				}

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			public override void OnCameraCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(Shader.PropertyToID(normals.name));
			}
		}

		private class ScreenSpaceOutlinePass : ScriptableRenderPass
		{

			private readonly Material screenSpaceOutlineMaterial;

			private RTHandle cameraColorTarget;
			private RenderTargetIdentifier temporaryBuffer;

			private readonly int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

			public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, ScreenSpaceOutlineSettings settings)
			{
				this.renderPassEvent = renderPassEvent;

				screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
				screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.outlineColor);
				screenSpaceOutlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);

				screenSpaceOutlineMaterial.SetFloat("_DepthThreshold", settings.depthThreshold);
				screenSpaceOutlineMaterial.SetFloat("_RobertsCrossMultiplier", settings.robertsCrossMultiplier);

				screenSpaceOutlineMaterial.SetFloat("_NormalThreshold", settings.normalThreshold);

				screenSpaceOutlineMaterial.SetFloat("_SteepAngleThreshold", settings.steepAngleThreshold);
				screenSpaceOutlineMaterial.SetFloat("_SteepAngleMultiplier", settings.steepAngleMultiplier);
			}

			public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
				temporaryTargetDescriptor.depthBufferBits = 0;
				cmd.GetTemporaryRT(temporaryBufferID, temporaryTargetDescriptor, FilterMode.Point);

				temporaryBuffer = new RenderTargetIdentifier("_TemporaryBuffer");

				cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
			}

			[Obsolete("Obsolete")]
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (!screenSpaceOutlineMaterial)
					return;

				CommandBuffer cmd = CommandBufferPool.Get();
				using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
				{
					Blit(cmd, cameraColorTarget, temporaryBuffer);
					Blit(cmd, temporaryBuffer, cameraColorTarget, screenSpaceOutlineMaterial);
				}

				context.ExecuteCommandBuffer(cmd);
				CommandBufferPool.Release(cmd);
			}

			public override void OnCameraCleanup(CommandBuffer cmd)
			{
				cmd.ReleaseTemporaryRT(temporaryBufferID);
			}
		}

		#endregion
	}
}