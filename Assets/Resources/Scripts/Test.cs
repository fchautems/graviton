using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Test : MonoBehaviour
{
    // Use this for initialization
        /*GameObject go;

        go=Instantiate(Resources.Load(path: "Prefabs/star"), Random.insideUnitSphere * 10.0f, Quaternion.identity) as GameObject;

        Vector3 minV = new Vector3(0.0f, 0.0f, 0.0f);

        Vector3 maxV = new Vector3(0.0f, 0.0f, 0.0f);

        Star s = new Star(go, minV, maxV, 23.0f);

        Tree<Star> arbre=new Tree<Star>(s);

        //arbre.addNodeToTree()
        // Create a 'Random' Tree: 3 Additional Levels, 4 Additional Nodes (a "wide" Tree)
        Tree<int> randomNumericTree = new Tree<int>(1000);
        randomNumericTree.populateTreeRandomly(3, 4);  // With (10,2) as Parameters, we have a "deep" Tree
        // Update a Node with Known Address
        int isu = randomNumericTree.updateNodeInTree(3, 6, 1, 535939115);
        // Ascend Nodes : always returns 'head' of Tree
        List<Tree<int>.TreeNode> randomTree1 = randomNumericTree.ascenByUnique(535939115);
        // Descend Nodes : always returns all Nodes
        List<Tree<int>.TreeNode> randomTree2 = randomNumericTree.traverseByUniqueFull(535939115);*/

        // Create a Tree of 'Strings' 
        /*Tree<string> myTreeString = new Tree<string>("Stepa");  /* A Sample Tree with 'String' Nodes  */
        /*myTreeString.addNodeToTree(2, 1, 1, "Tatiana");
        myTreeString.addNodeToTree(2, 2, 1, "Diana");
        myTreeString.addNodeToTree(2, 3, 1, "Ardalion");*/

        // Create a Real Tree for Testing with Known Values  : Regression Tested
        /*Tree<int> myTreeINT = new Tree<int>(1);

        /*  myTreeINT.addNodeToTree(1, 1, 0, 1);    Equivalent of Constructor : "owner" = 0  */
        /*myTreeINT.addNodeToTree(2, 1, 1, 2);   // Second Parent
        myTreeINT.addNodeToTree(2, 2, 1, 3);   // First Parent
        myTreeINT.addNodeToTree(2, 3, 1, 4);

        myTreeINT.addNodeToTree(3, 1, 1, 5);   // Second Child
        myTreeINT.addNodeToTree(3, 2, 1, 6);   // Second Child
        myTreeINT.addNodeToTree(3, 3, 1, 7);   // Second Child

        int s2 = myTreeINT.addNodeToTree(3, 3, 3, 619);  // DuplicateNode on L R
        int s1 = myTreeINT.addNodeToTree(3, 11, 3, 619); // OK

        myTreeINT.addNodeToTree(3, 8, 2, 13);  // Child !

        myTreeINT.addNodeToTree(3, 4, 2, 8);   // Child ! 2.2.1.3
        myTreeINT.addNodeToTree(3, 5, 2, 9);   // Child ! 2.2.1.3
        myTreeINT.addNodeToTree(3, 6, 2, 9);   // Duplicate Value

        myTreeINT.addNodeToTree(3, 7, 4, 10);  // Test No Parent Node

        myTreeINT.addNodeToTree(3, 7, 3, 10);  // Test Parent Node OK

        myTreeINT.addNodeToTree(4, 1, 2, 11);  // Test Depth/Width & Second Child
        myTreeINT.addNodeToTree(4, 2, 2, 12);
        myTreeINT.addNodeToTree(4, 2, 5, 66);  // 2nd Child    -> 3.5.2.9
        myTreeINT.addNodeToTree(4, 3, 5, 77);  // 2nd Child    -> 3.5.2.9

        myTreeINT.addNodeToTree(5, 1, 2, 33);  // Last Level
        myTreeINT.addNodeToTree(5, 2, 2, 34);

        myTreeINT.addNodeToTree(5, 5, 1, 37);  // Unordered Insert OK at Same Level 

        myTreeINT.addNodeToTree(5, 3, 3, 35);
        myTreeINT.addNodeToTree(5, 4, 3, 36);

        myTreeINT.addNodeToTree(3, 10, 2, 999); // Un-ordered Rank Insert
        //
        myTreeINT.addNodeToTree(3, 8, 1, 1999); // Un-ordered Rank Insert
        myTreeINT.addNodeToTree(3, 9, 1, 2999); // Un-ordered Rank Insert

        var theValueTest1 = myTreeINT.getNodeValueByAddress(3, 10, 2);       // get by unique address
        int[] theValueTest2 = myTreeINT.getNodeAddressFromUniqueValue(999);  // get by unique value symmetric

        // Alternative Way to Insert on Object:
        Tree<int>.TreeNode anode = new Tree<int>.TreeNode();
        anode.level = 3;
        anode.rank = 12;
        anode.owner_node = 1;
        anode.theValue = 3000;

        myTreeINT.addNodeToTree(anode.level, anode.rank, anode.owner_node, anode.theValue);

        int[] elem_num = myTreeINT.getValues();

        string[] elem_str = myTreeString.getValues();

        int theValue = myTreeINT.getNodeValueByAddress(3, 3, 1);

        int[] theValues = myTreeINT.getNodeAddressFromUniqueValue(theValue);  // = "7"

        int test = myTreeINT.updateNodeInTree(5, 1, 2, 3333);  // Was : (5, 1, 2, 33)

        int t = myTreeINT.getMaxDepth();

        int u = myTreeINT.getMaxWidthAtLevel(3);

        List<Tree<int>.TreeNode> nodes = myTreeINT.getAllNodes();

        myTreeINT.reinitializeTrees(); // Must Re-Initialize Static Tree

        // Walk Nodes (full scan of all child nodes)
        List<Tree<int>.TreeNode> Tree1 = myTreeINT.traverseByAddressFull(2, 1, 1, 2);

        // Walk Nodes (full scan of all child nodes, based on UNIQUE value)
        List<Tree<int>.TreeNode> TreeUniqueValue = myTreeINT.traverseByUniqueFull(2);

        List<Tree<string>.TreeNode> TreeString = myTreeString.traverseByAddressFull(1, 1, 0, "Stepa");

        bool isDeletedt1 = myTreeINT.deleteLeafNodeByUniqueValue(36);   // Successful Delete

        myTreeINT.reinitializeTrees();                            // Must Re-Initialize Static Tree

        Tree1 = myTreeINT.traverseByAddressFull(2, 2, 1, 3);     // One Node Deleted OK

        int test2 = myTreeINT.updateNodeInTree(5, 3, 3, 1111);

        myTreeINT.reinitializeTrees();                            // Must Re-Initialize Static Tree

        Tree1 = myTreeINT.traverseByAddressFull(2, 2, 1, 3);

        myTreeINT.reinitializeTrees();                            // Must Re-Initialize Static Tree

        List<Tree<int>.TreeNode> Tree2 = myTreeINT.traverseByAddressFull(2, 1, 1, 2);
        myTreeINT.reinitializeTrees();

        // Walk Nodes Single (level l+1 nodes only = "one level down")
        List<Tree<int>.TreeNode> Tree3 = myTreeINT.traverseByAddressOneLevel(2, 2, 1, 3);
        myTreeINT.reinitializeTrees();

        List<Tree<int>.TreeNode> Tree4 = myTreeINT.traverseByAddressOneLevel(2, 1, 1, 2);
        myTreeINT.reinitializeTrees();

        // Ascend Tree from Node (Walk Up)
        List<Tree<int>.TreeNode> Tree5 = myTreeINT.ascendByAddress(5, 2, 2, 34);
        System.Diagnostics.Debug.Assert(Tree5.Count == 4, "At Level 5, must have 4 unique Parent Nodes");

        myTreeINT.reinitializeTrees();

        // Ascend Just by Value
        List<Tree<int>.TreeNode> Tree6 = myTreeINT.ascenByUnique(34);
        System.Diagnostics.Debug.Assert(Tree5.Count == 4, "At Level 5, must have 4 unique Parent Nodes");

        myTreeINT.reinitializeTrees();

        // Delete Leaf Page by Value
        bool isDeletedOK = myTreeINT.deleteLeafNodeByUniqueValue(37);

        bool isDeletedNOK = myTreeINT.deleteLeafNodeByUniqueValue(6);

        // Delete an Entire Branch in Tree Starting from Bottom (leafs) to Top(owner nodes):
        myTreeINT.reinitializeTrees();

        Tree<int>.TreeNode testNode = new Tree<int>.TreeNode();
        testNode.level = 2;
        testNode.rank = 2;
        testNode.owner_node = 1;
        testNode.theValue = 3;

        bool isDel = myTreeINT.deleteBranchInTreeByNode(testNode, myTreeINT);
        myTreeINT.reinitializeTrees();
        Tree1 = myTreeINT.traverseByAddressFull(2, 2, 1, 3);  // Verify that Delete in  Branch is Correct*/
}
