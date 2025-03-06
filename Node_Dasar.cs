using System.Collections.Generic;
public abstract class Node
{
    public abstract bool Execute();
}

// Parallel Node untuk mengevaluasi semua child node secara bersamaan
public class ParallelNode : Node
{
    private List<Node> children = new List<Node>();

    public void AddChild(Node child)
    {
        children.Add(child);
    }

    public override bool Execute()
    {
        bool allSucceeded = true;
        foreach (Node child in children)
        {
            if (!child.Execute())
            {
                allSucceeded = false;
            }
        }
        return allSucceeded;
    }
}

// Sequence Node
public class Sequence : Node
{
    private List<Node> children = new List<Node>();

    public void AddChild(Node child)
    {
        children.Add(child);
    }

    public override bool Execute()
    {
        foreach (Node child in children)
        {
            if (!child.Execute())
            {
                return false;
            }
        }
        return true;
    }
}

// CheckNode untuk evaluasi kondisi
public class CheckNode : Node
{
    private System.Func<bool> condition;

    public CheckNode(System.Func<bool> condition)
    {
        this.condition = condition;
    }

    public override bool Execute()
    {
        return condition();
    }
}

// ActionNode untuk aksi tertentu
public class ActionNode : Node
{
    private System.Action action;

    public ActionNode(System.Action action)
    {
        this.action = action;
    }

    public override bool Execute()
    {
        action();
        return true;
    }
}