using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


//
// Tree Class:
// +==================================================================================+
// |----------------------------------------------------------------------------------|
// |std_cart.txt                                                                      |
// |----------------------------------------------------------------------------------|
// |==================================================================================|
// |Program Name  : Trees.cs                                                          |
// |Actual Vers.  : 1.1                                                               |
// |Author        : Steve Begelman (Steve.Begelman@gmail.com)                         |
// |Title         : Senior C++/C# Developper                                          |
// |                                                                                  |
// |Company       : Banque BPCE (Aix-en-Provence FRANCE)  IT-CE                       |
// |                "Direction de l’Edition - DSI Finance, Risques, Pilotage"         |
// |Create Date   : January 2015                                                      |
// |                                                                                  |
// |%%SCCS%% Rev. : Trees.cs                                                          |
// |SCCS Keywords : utility parse tree Tree                                           |
// |                                                                                  |
// |Dependencies  :  None : Stand-Alone '.dll' :  Uses "ITree.cs" Interface           |
// |Stored Procs. :  None :                                                           |
// |XML Config    :  None :                                                           |
// |----------------------------------------------------------------------------------|
// |Revision Hist.:  dd/mm/yy   |               Modifications                  |  By  |
// |----------------------------------------------------------------------------------|
// |                 05/01/2015  |   Basic Working Prototype & Driver          |  SB  |
// |                 07/01/2015  |   Coherency Checks on Update/Insert Added   |  SB  |
// |                 16/01/2015  |   Helper to Create 'Random" Tree    Added   |  SB  |
// |                 22/01/2015  |   IComparable 'sorter" added on Level       |  SB  |
// |                 28/01/2015  |   Delete an Entire Branch                   |  SB  |
// |----------------------------------------------------------------------------------|
// |              :                                                                   |
// |Roles         :  1) Inject Nodes to Tree (with validation)                        |
// |              :  2) Update/Delete Node in Tree                                    |
// |              :  3) Walk through Nodes in Tree : Upward and Downward              |
// |              :                                                                   |
// |Compiler      : Microsoft .NET 4.0   Uses:  LINQ                                  |
// |Compiler Opts.: /none                                                             |
// |                                                                                  |
// |Caveats       : Should work with any .Net CLR from 3.XX Forward                   |
// |              : Need to Support : LINQ, "nullable types" and Generics             |
// |Constraints   : reinitializeTrees() Method must be called for Each Parse of Tree  |
// |              : AND on Every "Delete" or "Update" Operation on Current Tree       |
// |              :                                                                   |
// +==================================================================================+
//

/*  TO DO : 1) Remove "System.Console.Writeline and replace with "Log4Net Framework"     */
/*          2) Extend Numeric Types : typeof(tinyint) typeof(long),...                   */

public class Tree<T> : System.Object, ITree<T> where T : IObject
{
    private enum Position
    {
        Level = 0,
        Rank = 1,
        OwnerNode = 2
    }

    private static System.Random _r = new System.Random();

    private static readonly object _sync = new object();

    private const int numeric_error = -1;

    private const bool isDebug = true;

    //void SampleMethod<T>() {}

    public struct TreeNode
    {
        public int level;          // Depth of Tree
        public int rank;           // Left to Right "Rank" = {1,2,3,...}
        public int owner_node;     // Node Owner Pointer:
        public T theValue;         // Generic Value Stored in Node (currently string or int)

        /* 
          Basic Architeture : For any Node of Type [Level,Rank,Owner,Value] , for example: [3,2,1,6456] we can
                              Infer that the Parent "level" is [level-1 = 2] , and "rank" of Parent = Owner = 1 
         */
    }

    /// <summary>
    ///      TO DO: Extend Code to Support Generic Sub-Types : smallint, tinyint, long, ...
    ///      This is  Simply "Boilerplate" Stuff
    /// </summary>

    private List<TreeNode> myTree = new List<TreeNode>();

    #region getters_setters

    private int _count;
    public int count
    {
        get
        {
            if (myTree == (List<TreeNode>)null)
            {
                this._count = 0;
                return this._count;
            }
            this._count = myTree.Count();
            return this._count;
        }
        set { this._count = value; }
    }

    private TreeNode _originvalue;
    public TreeNode originValue
    {
        get
        {
            _originvalue = (TreeNode)myTree[0];
            return this._originvalue;
        }
        set { this._originvalue = value; }
    }

    #endregion

    // Recursion Variables:
    private static int cur_level;
    private static int cur_rank;
    private static int cur_owner;
    private static T cur_value;

    private static int old_child_nodes = 0;
    private static int new_child_nodes = 0;

    public static int iterator = numeric_error;

    private static int max_levels;
    private static int max_ranks;

    private static List<TreeNode> recursionIntermediateNodes;

    public Tree(T param)
    {
        recursionIntermediateNodes = new List<TreeNode>();

        if (typeof(T) == typeof(int))
        {
            TreeNode treeItem = new TreeNode();
            treeItem.level = 1;
            treeItem.rank = 1;
            treeItem.owner_node = 0;    // Special Case: ONLY root node has no Parent: head of tree = 0

            T x = (T)(object)param;
            treeItem.theValue = x;

            myTree.Add(treeItem);
            int r = (int)(object)param;  // For debugger
        }

        if (typeof(T) == typeof(string))
        {
            TreeNode treeItem = new TreeNode();
            treeItem.level = 1;
            treeItem.rank = 1;
            treeItem.owner_node = 0;   // Special Case: ONLY root node has no Parent = 0

            T x = (T)(object)param;
            treeItem.theValue = x;

            myTree.Add(treeItem);

            string t = (string)(object)param;
        }

        if (typeof(T) == typeof(Star))
        {
            TreeNode treeItem = new TreeNode();
            treeItem.level = 1;
            treeItem.rank = 1;
            treeItem.owner_node = 0;   // Special Case: ONLY root node has no Parent = 0

            T x = (T)(object)param;
            treeItem.theValue = x;

            myTree.Add(treeItem);

            Star s = (Star)(object)param;
        }
    }

    // MUST be Called Before Every New Traversal : Reinitializes Static Tree Structures:
    // --------------------------------------------------------------------------------
    public void reinitializeTrees()
    {
        cur_level = 0;
        cur_rank = 0;
        cur_owner = 0;
        cur_value = default(T);

        old_child_nodes = 0;
        new_child_nodes = 0;

        iterator = numeric_error;

        max_levels = 0;
        max_ranks = 0;

        recursionIntermediateNodes = new List<TreeNode>();
    }

    // From Current Node : Iterate all Dependant Nodes from this Node:
    public List<Tree<T>.TreeNode> traverseByUniqueFull(T theValue)
    {
        Tree<T>.TreeNode theNode = getNodeFromUniqueValue(theValue);

        if (theNode.Equals(null)) return (List<Tree<T>.TreeNode>)null;

        return this.traverseByAddressFull(theNode.level, theNode.rank, theNode.owner_node, theValue);
    }

    // From Current Node : Iterate all Dependant Nodes from this Node:
    public List<Tree<T>.TreeNode> traverseByAddressFull(int? level, int? rank, int? owner_node, T theValue)
    {
        cur_level = (int)level;
        cur_rank = (int)rank;
        cur_owner = (int)owner_node;
        cur_value = (T)theValue;

        max_levels = getMaxDepth();
        max_ranks = getMaxWidthAtLevel((int)level);

        T valuated = isNodeValuated((int)level, (int)rank, (int)owner_node, theValue);

        var type = typeof(T);

        if (type == typeof(int))
        {
            int genericReturn = (int)(object)valuated;
            object y = (int)(object)valuated;
            if (genericReturn == 0) return (List<Tree<T>.TreeNode>)null;

            // Strong Optimization : No Need to Parse Tree if this Condition Exists:
            if ((level > max_levels) || (rank > max_ranks))
            {
                return (List<Tree<T>.TreeNode>)null;
            }
        }

        if (type == typeof(string))
        {
            string genericReturn = (string)(object)valuated;
            object y = (string)(object)valuated;
            if (genericReturn == (string)null) return (List<Tree<T>.TreeNode>)null;

            // Strong Optimization : No Need to Parse Tree if this Condition Exists:
            if ((level > max_levels) || (rank > max_ranks))
            {
                return (List<Tree<T>.TreeNode>)null;
            }
        }

        // Iterate Downward V
        while (old_child_nodes == new_child_nodes)
        {
            for (int i = 0; i < myTree.Count; i++)
            {
                if ((myTree[i].level == cur_level + 1) && (myTree[i].owner_node == cur_rank))
                {
                    TreeNode dependantItem = new TreeNode();

                    dependantItem.level = myTree[i].level;
                    dependantItem.owner_node = myTree[i].owner_node;

                    // Iterate Right ->                   
                    for (int j = 0; j < myTree.Count; j++)
                    {
                        if ((myTree[j].level == dependantItem.level) && (myTree[j].owner_node == dependantItem.owner_node))
                        {
                            dependantItem.rank = myTree[j].rank;
                            dependantItem.theValue = (T)myTree[j].theValue;

                            bool status = tempNodeUnique(recursionIntermediateNodes, dependantItem.level, dependantItem.rank, dependantItem.owner_node, dependantItem.theValue);

                            // Untested Optimization : check if "added nodes" > "leaf page count" = max_ranks
                            // If true, we are done with this 'for' loop and can Continue

                            if (status)
                            {
                                lock (_sync)
                                {
                                    try
                                    {
                                        recursionIntermediateNodes.Add(dependantItem);
                                        old_child_nodes = recursionIntermediateNodes.Count;
                                    }
                                    catch (Exception e)
                                    {
                                        System.Console.WriteLine("Technical Exception '{0}': Unable to Add Node to Tree @  '{1}' '{2}' '{3}' '{4}' ",
                                                                 e.Message + ":" + e.StackTrace + "\n", level.ToString(), rank.ToString(), owner_node.ToString(), theValue.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // Iterate Leafs:
            while (iterator < recursionIntermediateNodes.Count)
            {
                new_child_nodes = recursionIntermediateNodes.Count;
                iterator++;

                try
                {
                    // Recurse:
                    traverseByAddressFull(recursionIntermediateNodes[iterator].level,
                                                    recursionIntermediateNodes[iterator].rank,
                                                    recursionIntermediateNodes[iterator].owner_node,
                                                    recursionIntermediateNodes[iterator].theValue);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("_NONE_")) { };
                    old_child_nodes = numeric_error;
                    break;
                }
            }
        }
        return recursionIntermediateNodes;
    }

    // From Current Node : Ascend all Parent Nodes from this Node
    public List<Tree<T>.TreeNode> ascendByAddress(int? level, int? rank, int? owner_node, T theValue)
    {
        T valuated = isNodeValuated((int)level, (int)rank, (int)owner_node, theValue);

        var type = typeof(T);

        if (type == typeof(int))
        {
            int genericReturn = (int)(object)valuated;
            object y = (int)(object)valuated;
            if (genericReturn == 0) return (List<Tree<T>.TreeNode>)null;
        }

        if (type == typeof(string))
        {
            string genericReturn = (string)(object)valuated;
            object y = (string)(object)valuated;
            if (genericReturn == (string)null) return (List<Tree<T>.TreeNode>)null; ;
        }

        cur_level = (int)level;
        cur_rank = (int)rank;
        cur_owner = (int)owner_node;
        cur_value = (T)theValue;

        max_levels = getMaxDepth();
        max_ranks = getMaxWidthAtLevel((int)level);

        // Iterate Upward 
        for (int i = 0; i < myTree.Count; i++)
        {
            if ((myTree[i].level == cur_level - 1) && (cur_owner == myTree[i].rank))
            {
                TreeNode parentItem = new TreeNode();

                parentItem.level = myTree[i].level;
                parentItem.owner_node = myTree[i].owner_node;
                parentItem.rank = myTree[i].rank;
                parentItem.theValue = (T)myTree[i].theValue;

                bool status = tempNodeUnique(recursionIntermediateNodes, parentItem.level, parentItem.rank, parentItem.owner_node, parentItem.theValue);

                if (status)
                {
                    lock (_sync)
                    {
                        try
                        {
                            recursionIntermediateNodes.Add(parentItem);
                            iterator++;
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("Technical Exception '{0}': Unable to Ascend Node in Tree @  '{1}' '{2}' '{3}' '{4}' ",
                                                     e.Message + ":" + e.StackTrace, level.ToString() + "\n", level.ToString(), rank.ToString(), owner_node.ToString(), theValue.ToString());
                        }
                    }
                }
                // Iterate Ascendant Levels
                if ((iterator) <= (max_levels - 1))
                {
                    try
                    {
                        // Recurse:
                        if ((max_levels - iterator - 1) >= 0)  // Protect Index
                        {
                            ascendByAddress(recursionIntermediateNodes[iterator].level,
                                                recursionIntermediateNodes[iterator].rank,
                                                recursionIntermediateNodes[iterator].owner_node,
                                                recursionIntermediateNodes[iterator].theValue);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("_NONE_")) { };
                        break;
                    }
                }
            }
            // Optimizations: 
            if (recursionIntermediateNodes.Count > max_levels) break;
            if ((max_levels - iterator - 1) == 0) break;
        }
        return recursionIntermediateNodes;
    }

    // Unused :
    //public static IComparer<Tree<T>.TreeNode> sortLevelAscending()
    //{
    //    return (IComparer<Tree<T>.TreeNode>) new sortLevelAscendingHelper<T>();
    //}
    ////
    //public static IComparer<Tree<T>.TreeNode> sortLevelDescending()
    //{
    //    return (IComparer<Tree<T>.TreeNode>)new sortLevelDescendingHelper<T>();
    //}

    // From Current Node : Ascend all Parent Nodes from this Node by Key
    public List<Tree<T>.TreeNode> ascenByUnique(T theValue)
    {
        Tree<T>.TreeNode theNode = getNodeFromUniqueValue(theValue);

        if (theNode.Equals(null)) return (List<Tree<T>.TreeNode>)null;

        return this.ascendByAddress(theNode.level, theNode.rank, theNode.owner_node, theValue);
    }

    // From Cuurent Node : Get Child Nodes from this Node One Level Down
    public List<Tree<T>.TreeNode> traverseByUniqueOneLevel(T theValue)
    {
        Tree<T>.TreeNode theNode = getNodeFromUniqueValue(theValue);

        if (theNode.Equals(null)) return (List<Tree<T>.TreeNode>)null;

        return this.traverseByAddressOneLevel(theNode.level, theNode.rank, theNode.owner_node, theValue);
    }

    // From Cuurent Node : Get Child Nodes from this Node One Level Down
    public List<Tree<T>.TreeNode> traverseByAddressOneLevel(int? level, int? rank, int? owner_node, T theValue)
    {
        T valuated = isNodeValuated((int)level, (int)rank, (int)owner_node, theValue);

        var type = typeof(T);

        if (type == typeof(int))
        {
            int genericReturn = (int)(object)valuated;
            object y = (int)(object)valuated;
            if (genericReturn == 0) return (List<Tree<T>.TreeNode>)null;
        }

        if (type == typeof(string))
        {
            string genericReturn = (string)(object)valuated;
            object y = (string)(object)valuated;
            if (genericReturn == (string)null) return (List<Tree<T>.TreeNode>)null; ;
        }

        cur_level = (int)level;
        cur_rank = (int)rank;
        cur_owner = (int)owner_node;
        cur_value = (T)theValue;

        max_levels = getMaxDepth();
        max_ranks = getMaxWidthAtLevel((int)level);

        // Iterate Downward V
        for (int i = 0; i < myTree.Count; i++)
        {
            if ((myTree[i].level == cur_level + 1) && (myTree[i].owner_node == cur_rank))
            {
                TreeNode dependantItem = new TreeNode();

                dependantItem.level = myTree[i].level;
                dependantItem.owner_node = myTree[i].owner_node;

                // Iterate Right ->                   
                for (int j = 0; j < myTree.Count; j++)
                {
                    if ((myTree[j].level == dependantItem.level) && (myTree[j].owner_node == dependantItem.owner_node))
                    {
                        dependantItem.rank = myTree[j].rank;
                        dependantItem.theValue = (T)myTree[j].theValue;

                        bool status = tempNodeUnique(recursionIntermediateNodes, dependantItem.level, dependantItem.rank, dependantItem.owner_node, dependantItem.theValue);

                        if (status)
                        {
                            lock (_sync)
                            {
                                try
                                {
                                    recursionIntermediateNodes.Add(dependantItem);
                                }
                                catch (Exception e)
                                {
                                    System.Console.WriteLine("Technical Exception '{0}': Unable to Add Node to Tree @  '{1}' '{2}' '{3}' '{4}' ",
                                    e.Message + ":" + e.StackTrace + "\n", level.ToString(), rank.ToString(), owner_node.ToString(), theValue.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }
        return recursionIntermediateNodes;
    }

    private bool tempNodeUnique(List<TreeNode> nodes, int level, int rank, int owner, T value)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if ((nodes[i].level == level) && (nodes[i].rank == rank) && (nodes[i].owner_node == owner) && (nodes[i].level == level))
                return false;
        }
        return true;
    }

    public List<TreeNode> getAllNodes()
    {
        List<TreeNode> theList = new List<TreeNode>();

        for (int x = 0; x < myTree.Count(); x++)
        {
            theList.Add(myTree[x]);
        }
        if (theList.Count == 0) return (List<TreeNode>)null;
        return theList;
    }

    // Via LINQ
    public int getMaxDepth()
    {
        var depth = this.myTree.Max(x => x.level);
        return (int)depth;
    }

    // Via LINQ
    public int getMaxWidthAtLevel(int level)
    {
        var width = this.myTree.Where(p => p.level == level).Select(x => x.rank).Count();
        return (int)width;
    }

    public int countElementsInTree()
    {
        if (myTree == (List<TreeNode>)null) return 0;
        return myTree.Count();
    }

    public T[] getValues()
    {
        List<T> theList = new List<T>();

        for (int x = 0; x < myTree.Count(); x++)
        {
            theList.Add(myTree[x].theValue);
        }

        if (theList.Count == 0) return (T[])null;

        return theList.ToArray();
    }

    public List<T> getValuesList()
    {
        List<T> theList = new List<T>();

        for (int x = 0; x < myTree.Count(); x++)
        {
            theList.Add(myTree[x].theValue);
        }

        if (theList.Count == 0) return (List<T>)null;

        return theList;
    }

    public int addNodeToTree(int x, int y, int z, T theValue)
    {
        TreeNode treeItem = new TreeNode();

        treeItem.level = x;
        treeItem.rank = y;
        treeItem.owner_node = z;
        treeItem.theValue = theValue;

        // Does Key have null Value: 
        string tmp = theValue.ToString();

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        // Positive Integers in Address:
        if ((x < 0) || (y < 0) || (z < 0)) return 0;

        // For Unique Key Values in Each Node: uncomment
        /*
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if (myTree[elem].theValue.ToString() == treeItem.theValue.ToString())
            {
                return 0;
            }
        }
         */

        Debug.Log("count : " + myTree.Count());
        // Unique (Level + Rank)
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == treeItem.level) &&
                (myTree[elem].rank == treeItem.rank))
            {
                //le noeud courant
                TreeNode currentNode = myTree[elem];
                Vector3 minV = theValue.getMinV();
                Vector3 maxV = theValue.getMaxV();

                //si ce n'est pas un groupe de Star
                if (!myTree[elem].theValue.isGroup())
                {
                    //(Star)theValue.GetType.
                    // level + 1, rank + cadran, rank

                    // set new min max et cadran à nouvelle valeur
                    theValue.newMinMaxCadran();

                    // set new min max et cadran à valeur du noeud
                    myTree[elem].theValue.newMinMaxCadran();

                    Debug.Log("DOUBLON SANS GROUPE");

                    // ajoute au niveau du dessous la valeur existante à ce noeud
                    addNodeToTree(x + 1, y + myTree[elem].theValue.getCadran(), y, theValue);

                    // set la valeur du noeud courant qui devient un groupe de Star
                    myTree[elem].theValue.newStar(minV, maxV, theValue.getM() + myTree[elem].theValue.getM()); // = (T)(object)theValue.newStar(minV, maxV, theValue.getM() + myTree[elem].theValue.getM());

                    Debug.Log("DOUBLON SANS GROUPE");

                }
                else
                {
                    Debug.Log("DOUBLON AVEC GROUPE");
                    currentNode.theValue.addM(theValue.getM()); //= (T)(object)theValue.newStar(minV, maxV, theValue.getM() + myTree[elem].theValue.getM());
                }

                Debug.Log("MON DIEU");
                // ajoute au niveau du dessous la valeur reçue en paramètre
                addNodeToTree(x + 1, y + theValue.getCadran(), y, theValue);

                return 0;
            }
        }

        // Does this Node Have Owner-Parent Node:
        int owner_level = x - 1;
        int owner_rank = z - 0;

        bool has_parent = false;

        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            Debug.Log("elem : " + elem);
            Debug.Log("level : " + myTree[elem].level);
            Debug.Log("owner_level " + owner_level);
            Debug.Log("rank : " + myTree[elem].rank);
            Debug.Log("owner_rank " + owner_rank);

            if ((myTree[elem].level == owner_level) &&
                (myTree[elem].rank == owner_rank))
            {
                has_parent = true;
                break;
            }
        }

        if (!has_parent)
        {
            if (isDebug)
            {
                Debug.Log("PAS DE PARENT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                System.Console.WriteLine("No Parent: Unable to Add Node to Tree @  '{0}' '{1}' '{2}' '{3}' ",
                                          x.ToString(), y.ToString(), z.ToString(), theValue.ToString());
            }
            return 0;
        }

        lock (_sync)
        {
            try
            {
                myTree.Add(treeItem);
            }
            catch (Exception e)
            {
                Debug.Log("UNABLE TO ADD NODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                System.Console.WriteLine("Technical Exception '{0}': Unable to Add Node to Tree @  '{1}' '{2}' '{3}' '{4}' ",
                                          e.Message + ":" + e.StackTrace, x.ToString(), y.ToString(), z.ToString(), theValue.ToString());
                return 0;
            }
        }
        Debug.Log("INSERTION REUSSIE");
        return 1;  // Success
    }

    public T getNodeValueByAddress(int x, int y, int z)
    {
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == x) &&
                (myTree[elem].rank == y) &&
                (myTree[elem].owner_node == z))
            {
                return myTree[elem].theValue;
            }
        }
        return default(T);
    }

    public T isNodeValuated(int x, int y, int z, T value)
    {
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == x) &&
                (myTree[elem].rank == y) &&
                (myTree[elem].owner_node == z) &&
                (myTree[elem].theValue.ToString() == value.ToString())
                )
            {
                return myTree[elem].theValue;
            }
        }
        return default(T);
    }

    public int[] getNodeAddressFromUniqueValue(T inputValue)
    {
        int[] coordinates = new int[3];

        bool isunique = getIsUniqueValue(inputValue);

        coordinates[(int)Position.Level] = numeric_error;
        coordinates[(int)Position.Rank] = numeric_error;
        coordinates[(int)Position.OwnerNode] = numeric_error;

        if (!isunique) return coordinates;

        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if (myTree[elem].theValue.ToString() == inputValue.ToString())
            {
                coordinates[(int)Position.Level] = myTree[elem].level;
                coordinates[(int)Position.Rank] = myTree[elem].rank;
                coordinates[(int)Position.OwnerNode] = myTree[elem].owner_node;

                return coordinates;
            }
        }
        return coordinates;
    }

    public bool getIsUniqueValue(T inputValue)
    {
        int unique = 0;

        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if (myTree[elem].theValue.ToString() == inputValue.ToString())
            {
                unique++;
                if (unique >= 2) break;
            }
        }
        if (unique == 1) return true;
        else return false;
    }

    public TreeNode getNodeFromUniqueValue(T inputValue)
    {
        TreeNode anode = new TreeNode();

        bool isunique = getIsUniqueValue(inputValue);

        if (!isunique)
        {
            return anode;
        }

        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if (myTree[elem].theValue.ToString() == inputValue.ToString())
            {
                anode.level = myTree[elem].level;
                anode.rank = myTree[elem].rank;
                anode.owner_node = myTree[elem].owner_node;
                anode.theValue = (T)inputValue;

                break;
            }
        }
        return anode;
    }

    public int updateNodeInTree(int x, int y, int z, T modifiedValue)
    {
        TreeNode treeItem = new TreeNode();

        bool found = false;

        treeItem.level = x;
        treeItem.rank = y;
        treeItem.owner_node = z;
        treeItem.theValue = modifiedValue;

        // Does Node Element Have Existant  Node Address:
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == treeItem.level) &&
                (myTree[elem].rank == treeItem.rank) &&
                (myTree[elem].owner_node == treeItem.owner_node))
            {
                found = true;
                myTree.RemoveAt(elem);
                break;
            }
        }

        int status = 0;

        if (found) status = addNodeToTree(x, y, z, modifiedValue);

        if (!isDebug)
        {
            if (status == 0) return 0;
        }

        if (isDebug)
        {
            if (status == 0)
            {
                System.Console.WriteLine("No Parent: Unable to Update Node to Tree @ '{0}' '{1}' '{2}' '{3}' ",
                                          x.ToString(), y.ToString(), z.ToString(), modifiedValue.ToString());
                return 0;
            }
        }
        return 1;
    }

    public bool deleteLeafNodeByAddress(int x, int y, int z)
    {
        TreeNode treeItem = new TreeNode();

        treeItem.level = x;
        treeItem.rank = y;
        treeItem.owner_node = z;

        T existantNode = getNodeValueByAddress(x, y, z);

        if (existantNode == null)
        {
            if (isDebug)
            {
                System.Console.WriteLine("Non Existant Node Leaf Address  @ '{0}' '{1}' '{2}' ", x.ToString(), y.ToString(), z.ToString());
            }
            return false;
        }

        int child_level = treeItem.level + 1; // Current Level ++
        int child_owner = treeItem.rank;      // Child Points to Rank

        // Check for Child Nodes:
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == child_level) && (myTree[elem].owner_node == child_owner))
            {
                if (isDebug)
                {
                    System.Console.WriteLine("Not a Leaf Node: Has Dependants  @  '{0}' '{1}' '{2}'  ", x.ToString(), y.ToString(), z.ToString());
                }
                return false;
            }
        }
        //

        // Does Node Element Have Valid Node Address:
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == treeItem.level) &&
                (myTree[elem].rank == treeItem.rank) &&
                (myTree[elem].owner_node == treeItem.owner_node))
            {
                myTree.RemoveAt(elem);
                break;
            }
        }
        return true;
    }

    public bool deleteLeafNodeByUniqueValue(T value)
    {
        TreeNode treeItem = new TreeNode();

        int[] values = getNodeAddressFromUniqueValue(value);
        if (values[0] == numeric_error) return false;

        treeItem.level = values[(int)Position.Level];
        treeItem.rank = values[(int)Position.Rank];
        treeItem.owner_node = values[(int)Position.OwnerNode];

        T existantNode = getNodeValueByAddress(treeItem.level, treeItem.rank, treeItem.owner_node);

        if (existantNode == null)
        {
            if (isDebug)
            {
                System.Console.WriteLine("Non-Existant Node Leaf Value @ '{0}'  ", value.ToString());
            }
            return false;
        }

        int child_level = treeItem.level + 1; // Current Level ++
        int child_owner = treeItem.rank;      // Child Points to Rank

        // Check for Child Nodes:
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == child_level) && (myTree[elem].owner_node == child_owner))
            {
                if (isDebug)
                {
                    System.Console.WriteLine("Not a Leaf Node Value: Has Dependants @ '{0}' ", value.ToString());
                }
                return false;
            }
        }

        // Does Node Element Have Valid Node Address:
        for (int elem = 0; elem < myTree.Count(); elem++)
        {
            if ((myTree[elem].level == treeItem.level) &&
                (myTree[elem].rank == treeItem.rank) &&
                (myTree[elem].owner_node == treeItem.owner_node))
            {
                myTree.RemoveAt(elem);
                break;
            }
        }
        return true;
    }

    private static int getRandomInt()
    {
        int random = _r.Next();
        return random;
    }

    public bool deleteBranchInTreeByNode(TreeNode value, Tree<T> myTree)
    {
        List<Tree<T>.TreeNode> branch = myTree.traverseByAddressFull(value.level, value.rank, value.owner_node, (T)value.theValue);

        try
        {
            // Reverse: Bottom to Top, no Need to Sort
            for (int iterator = branch.Count - 1; iterator >= 0; iterator--)
            {
                bool b = myTree.deleteLeafNodeByUniqueValue((T)branch[iterator].theValue);

                myTree.reinitializeTrees();
            }
        }
        catch (Exception e)
        {
            if (isDebug)
            {
                System.Console.WriteLine("Delete of Branch Failed  @ '{0}' ", value.theValue + " " + e.Message);
            }
            return false;
        }
        return true;
    }

    // Populate Tree with Random Numeric Values: Usefull for Quick Test, Not part of Interface:
    //
    public bool populateTreeRandomly(int levels_added, int ranks_per_level)
    {
        var newValue = 0;
        int sanity = 1;

        const int SANITY_VAL = 10000;    // 10 000 Values requires about 10 seconds to Insert

        try
        {
            for (int level = 1; level <= levels_added; level++)
            {
                double nodes_at_level = Math.Pow(ranks_per_level, level);

                double parent_nodes = Math.Pow(ranks_per_level, level - 1);

                for (int rank = 1; rank <= (int)nodes_at_level; rank++)
                {
                    for (int pnode = 1; pnode <= parent_nodes; pnode++)
                    {
                        newValue = getRandomInt();

                        T stored_numeric = (T)(object)newValue;

                        int isInsert = addNodeToTree(level + 1, (int)rank, (int)pnode, stored_numeric);

                        sanity++;

                        if (sanity > SANITY_VAL)
                        {
                            if (isDebug)
                            {
                                System.Console.WriteLine("Max Sample Values Reached @ '{0}' Counter '{1}' ", newValue.ToString(), sanity.ToString());
                            }
                            return true;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (isDebug)
            {
                System.Console.WriteLine("Technical Population Exception @ '{0}' Counter '{1}' ", newValue.ToString(), sanity.ToString());
                return false;
            }
        }
        return true;
    }
}