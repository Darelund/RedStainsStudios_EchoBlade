using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Tree
{
    public Node Root;

    public int Depth()
    {
        return Depth(Root, 0);
    }
    public void ShowSkillTree(Node tree)
    {
        LevelOrder(tree, out List<Node> nodesOrder);
        foreach (var node in nodesOrder)
        {
            node.gameObject.SetActive(true);
        }
    }


    private int Depth(Node node, int currentDepth)
    {
        int depth = 0;
        if (node == null)
            return currentDepth;

        if (node.Children == null)
            return currentDepth;

        for (int i = 0; i < node.Children.Length; i++)
        {
            depth = Mathf.Max(Depth(node.Children[i], currentDepth + 1), currentDepth);
        }
        return depth;
    }
    public int Count1()
    {
        return Count1(Root, 1);
    }
    private int Count1(Node node, int currentDepth)
    {
        //int depth = 0;
        if (node == null)
            return currentDepth;

        if (node.Children == null)
            return currentDepth;

        for (int i = 0; i < node.Children.Length; i++)
        {
            currentDepth = Mathf.Max(Count1(node.Children[i], currentDepth + 1), currentDepth);
        }
        return currentDepth;
    }

    public void LevelOrder(Node root, out List<Node> nodesOrder)
    {
        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(root);
        nodesOrder = new List<Node>();
        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();
            if (current.IsUnlocked)
                nodesOrder.Add(current);
            for (int i = 0; i < current.Children.Length; i++)
            {
                if (current.Children[i] != null)
                    queue.Enqueue(current.Children[i]);
            }

        }
    }
    public static Node CreateRandomTree(int currentDepth, int maxDepth, int maxChildren)
    {
        Node node = new Node()
        {
            Value = default
        };

        if (currentDepth < maxDepth)
        {
            int childCount = Random.Range(0, maxChildren + 1);
            if (childCount > 0)
            {
                node.Children = new Node[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    node.Children[i] = CreateRandomTree(currentDepth + 1, maxDepth, maxChildren);
                }
            }
        }
        return node;
    }
}
