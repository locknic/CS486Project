using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public Node connected1;
    public Node connected2;
    public Node connected3;
    public Node connected4;
    public Node connected5;
    public Node connected6;
    public Node connected7;
    public Node connected8;

    public int food;
    public int thirst;
    public int energy;
    public int comfort;
    public int hygene;
    public int fun;

	private List<Node> edges;

    void Start()
    {
        edges = new List<Node>();

        if (connected1 != null)
        {
			edges.Add(connected1);
        }
        if (connected2 != null)
        {
			edges.Add(connected2);
        }
        if (connected3 != null)
        {
			edges.Add(connected3);
        }
        if (connected4 != null)
        {
			edges.Add(connected4);
        }
       	if (connected5 != null)
        {
			edges.Add(connected5);
        }
        if (connected6 != null)
        {
			edges.Add(connected6);
        }
        if (connected7 != null)
        {
			edges.Add(connected7);
        }
        if (connected8 != null)
        {
			edges.Add(connected8);
        }
    }

	public Stack<Node> FindShortest(Node destination, List<Node> previouslyVisited)
    {
		Stack<Node> currentShortest = null;
		List<Node> newPreviouslyVisited = new List<Node> ();
		newPreviouslyVisited.AddRange (previouslyVisited);
		newPreviouslyVisited.Add (this);

		foreach (Node nextNode in edges)
        {
            if (nextNode == destination)
            {
				currentShortest = new Stack<Node> ();
				currentShortest.Push (destination);
				return currentShortest;
            }

			if (!newPreviouslyVisited.Contains(nextNode))
            {
				Stack<Node> currentPath = nextNode.FindShortest (destination, newPreviouslyVisited);
				if (currentPath != null) 
				{
					currentPath.Push (nextNode);

					if (currentShortest != null) 
					{
						if (currentPath.Count < currentShortest.Count) 
						{
							currentShortest = currentPath;
						}
					} 
					else 
					{
						currentShortest = currentPath;
					}
				}
            }
        }

		return currentShortest;
    }

}