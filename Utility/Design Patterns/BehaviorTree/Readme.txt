#========== Guide To Behavior Tree ==========#

#=== Nodes ===#

1)Sequence:
	Runs leaf nodes in the sequence they are added or as per priority provided. On Sequence Completed return true and if any leaf node fails its terminated.

2)RSequence:
	Runs leaf nodes in a random order every time.On Sequence Completed return true and if any leaf node fails its terminated.

3)Parallel:
	Runs leaf nodes in parallel and requires policy to define condition on which its cosidered passed.
	Policies are OneSucceed(As soon as 1 leaf node is successful this node returns successful) or All Succeed(Only when all nodes succeed this nodes return successful).

4)Selector:
	Acts as if,else if and else.Goes through the leaf node and stops if one of its leaf node returns success.

5)PSelector:
	Ordered Selector.Runs in order provided. Same as Selector.

6)RSelector:
	Randomized Selector.Runs in random order each time. Same as Selector

7)Conditional:
	Executes Leaf if the provided condition returns true.

8)Cooldown:
	Executes Leaf node only after a provided cooldown.
	
9)DepSequence:
	Dependency Sequence. Executes another sub behavior tree and returns sucess or failure. Similar to conditional node but used for more complex condtions.

10)WaitForCondition:
	Holds any further action of behavior tree until the condition is satisfied
    Note:This class can NEVER have more than 1 CHILD NODE
    Child node can be anything composite, decorator, service or task

11)WaitForSeconds:
	Halts progress of behavior tree for provided time.

12)Leaf:
	The execution node. It is the node that contains behavior or program to be executed.