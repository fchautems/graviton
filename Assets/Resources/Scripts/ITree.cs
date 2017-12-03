using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Implementation File : TreeImplementation.cs
// ------------------------------------------
//using System;
//using System.Collections.Generic;
//using System.Linq;

// All Interface Members:
public interface ITree<T> where T:IObject
{
    int countElementsInTree();

    int count { get; set; }
    Tree<T>.TreeNode originValue { get; set; }

    T[] getValues();
    List<T> getValuesList();

    int addNodeToTree(int level, int rank, int owner_node, T newElement);
    int updateNodeInTree(int level, int rank, int owner_node, T modifiedElement);

    bool getIsUniqueValue(T inputValue);

    Tree<T>.TreeNode getNodeFromUniqueValue(T inputValue);

    T getNodeValueByAddress(int level, int rank, int owner_node);

    int[] getNodeAddressFromUniqueValue(T inputValue);

    bool deleteLeafNodeByAddress(int level, int rank, int owner_node);
    bool deleteLeafNodeByUniqueValue(T value);

    List<Tree<T>.TreeNode> getAllNodes();


    int getMaxDepth();
    int getMaxWidthAtLevel(int level);

    List<Tree<T>.TreeNode> traverseByAddressFull(int? level, int? rank, int? owner_node, T theValue);
    List<Tree<T>.TreeNode> traverseByUniqueFull(T theValue);

    List<Tree<T>.TreeNode> traverseByAddressOneLevel(int? level, int? rank, int? owner_node, T theValue);
    List<Tree<T>.TreeNode> traverseByUniqueOneLevel(T theValue);

    List<Tree<T>.TreeNode> ascendByAddress(int? level, int? rank, int? owner_node, T theValue);
    List<Tree<T>.TreeNode> ascenByUnique(T theValue);

    bool deleteBranchInTreeByNode(Tree<T>.TreeNode value, Tree<T> myTree);

    void reinitializeTrees();
}


