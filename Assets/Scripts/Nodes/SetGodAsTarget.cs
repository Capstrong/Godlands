using System.Collections;
using BehaviorTree;

class SetGodAsTarget : LeafNode
{
	Hashtable _data;

	public override void InitSelf( Hashtable data )
	{
		_data = data;
	}

	public override NodeStatus TickSelf()
	{
		_data["target"] = _data["god"];
		return NodeStatus.SUCCESS;
	}
}
