using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synthesize.CliquePartition
{
    /// <summary>
    /// Ported from the clique_partition.c file in this repo.
    /// </summary>
    public class CliqueHelper
    {
        public const int Unknown = -12345;
        public const int Maxcliques = 200;

        public const int CliqueUnknown = -12345;
        public const int CliqueTrue = 100;
        public const int CliqueFalse = 110;

        public class Clique
        {
            public int[] Members = new int[Maxcliques];/* members of the clique */
            public int Size;/* number of members in the clique */
        }
        public Clique[] CliqueSet = Enumerable
            .Range(0, Maxcliques)
            .Select(index => new Clique())
            .ToArray();
        
        public bool EnableConsolePrinting { get; set; } = true;
        private void print(string text, params int[] numbers)
        {
            if (EnableConsolePrinting)
            {
                var index = 0;
                text = Regex.Replace(text, "%d", match => index < numbers.Length ? numbers[index++].ToString() : "%d");
                Console.Write(text);
            }
        }
        private void exit(int exitCode)
        {
            throw new InvalidProgramException("The program should exit with code: " + exitCode);
        }
        // ReSharper disable once UnusedParameter.Local
        private void assert(bool test)
        {
            if (!test)
            {
                throw new InvalidProgramException("The assert failed.");
            }
        }
        private void inputSanityCheck(int[][] compat, int arrayDimension)
        {
            /* Verifies whether the compat array passed is valid array
             *  (1) Is each array entry =0 or 1?
             *  (2) Is the matrix symmetric
             * Note that diagonal entries can be either 0 or 1.
             */

            print(" Checking the sanity of the input..");

            for (var i = 0; i < arrayDimension; i++)
            {
                for (var j = 0; j < arrayDimension; j++)
                {
                    if ((compat[i][j] != 1) && (compat[i][j] != 0))
                    {
                        print(" %d \n", compat[i][j]);
                        print("The value of an array element is other than 1 or 0. Aborting..\n");
                        exit(0);
                    }
                    if (compat[i][j] != compat[j][i])
                    {
                        print("The compatibility array is NOT symmetric at (%d,%d) and (%d,%d)! Aborting..\n ", i, j, j, i);
                        exit(0);
                    }
                    print(".");
                }
            }

            print("Done.\n");
        }
        private void outputSanityCheck(int[][] localCompat, int[][] compat)
        {
            /* 
             * Verifies the results of the heuristic.
             * for each clique do
             *   if the clique size is UNKNOWN
             *      break
             *   else
             *     for every pair of members x and y in clique do
             *       assert  compat[x][y] = 1 and compat[y][x] = 1
             *     end for
             *   end if
             * end for
             */
            print("\n Verifying the results of the clique partitioning algorithm..");
            for (var i = 0; i < Maxcliques; i++)
            {
                if (CliqueSet[i].Size != Unknown)
                {
                    assert(CliqueSet[i].Size > 0);
                    for (var j = 0; j < CliqueSet[i].Size; j++)
                    {
                        for (var k = 0; k < CliqueSet[i].Size; k++)
                        {
                            if (j != k)
                            {
                                var member1 = CliqueSet[i].Members[j];
                                var member2 = CliqueSet[i].Members[k];

                                assert(compat[member1][member2] == 1);
                                assert(compat[member2][member1] == 1);
                                assert(localCompat[member2][member1] == 1);
                                assert(localCompat[member2][member1] == 1);
                                print(".");
                            }
                        }
                    }
                }
            }
            print("..Done.\n");
        }
        private void makeALocalCopy(int[][] localCompat, int[][] compat, int nodesize)
        {
            for (var i = 0; i < nodesize; i++)
            {
                for (var j = 0; j < nodesize; j++)
                {
                    localCompat[i][j] = compat[i][j];
                }
            }
        }
        private int getDegreeOfANode(int x, int nodesize, int[][] localCompat, int[] nodeSet)
        {
            var nodeDegree = 0;
            for (var j = 0; j < nodesize; j++) /* compute node degrees */
            {
                if (nodeSet[j] != CliqueUnknown)
                {
                    if (x != j)
                    {
                        nodeDegree += localCompat[x][j];
                    }
                }
            }
            return nodeDegree;
        }
        private int selectNewNode(int[][] localCompat, int nodesize, int[] nodeSet)
        {
            /*    if a node with priority, then pick that node 
             *      else a node with highest degree
             *        if multiple nodes then pick a node
             *           with highest neighbor wt
             *             if multiple pick one randomly.   
             */
            int index;
            var degrees = new int[nodesize][];
            var maxNode = CliqueUnknown;

            for (var i = 0; i < nodesize; i++) /* initialize the degrees matrix */
            {
                degrees[i] = new int[nodesize];
                for (var j = 0; j < nodesize; j++)
                {
                    /* j dimension = node with degree=i */
                    degrees[i][j] = CliqueUnknown;
                }
            }

            var currMaxDegree = 0;
            for (var i = 0; i < nodesize; i++) /* for each node do */
            {
                if (nodeSet[i] != CliqueUnknown) /* if the node is still in N */
                {
                    var currNodeDegree = getDegreeOfANode(i, nodesize, localCompat, nodeSet);

#if DEBUG
                    print(" node=%d curr_node_degree = %d \n", i, currNodeDegree);
#endif
                    if (currNodeDegree > currMaxDegree)
                    {
                        currMaxDegree = currNodeDegree;
                    }

                    /* append to a list of nodes with degree=curr_node_degree */
                    index = 0;
                    while (degrees[currNodeDegree][index] != CliqueUnknown)
                    {
                        index++;
                    }
                    degrees[currNodeDegree][index] = i; /* register this node */
                }
            }

            /* for debugging purposes.. 
            for (i = 0; i < nodesize; i++) 
            {  
                for (j = 0; j < nodesize; j++)  
                {  
                    print(" %d %d %d \n", i, j, degrees[i][j]);
                }
            }
            */

            if (degrees[currMaxDegree][1] == CliqueUnknown) /* only one max node */
            {
                maxNode = degrees[currMaxDegree][0];
            }
            else if (degrees[currMaxDegree][1] != CliqueUnknown) /* multiple max nodes */
            {
                var maxCurrNeighborsWt = 0;

                for (index = 0; index < nodesize; index++)
                /* go through all nodes with curr_max_degree*/
                {
                    if (degrees[currMaxDegree][index] != CliqueUnknown)
                    /* not end of list of the nodes with curr_max_degree */
                    {
                        var currNeighborsWt = 0;
                        var currNode = degrees[currMaxDegree][index];

                        /* get cumulative neighbor weight for this node */
                        currNeighborsWt += getDegreeOfANode(currNode, nodesize, localCompat, nodeSet);
#if DEBUG
                        print("curr_node = %d curr_neighbors_wt=%d\n", currNode, currNeighborsWt);
#endif
                        /* Is the local_compat, node_set consistent? */
                        if (currNeighborsWt >= maxCurrNeighborsWt)
                        {
                            maxCurrNeighborsWt = currNeighborsWt;
                            maxNode = currNode;
                        }
                    }
                }
            }
#if DEBUG
            print(" curr_max_degree = %d max_node= %d\n", currMaxDegree, maxNode);
#endif

            return maxNode;
        }
        private int formSetY(int[] setY, int[] currentClique, int[][] localCompat, int nodesize, int[] nodeSet)
        {
            var index = 0;

            /* reset set_Y */
            for (var i = 0; i < nodesize; i++)
            {
                setY[i] = CliqueUnknown;
            }

            for (var i = 0; i < nodesize; i++)
            {
                var compatibility = CliqueTrue;
                if (nodeSet[i] != CliqueUnknown)
                {
                    for (var j = 0; j < nodesize; j++)
                    {
                        if (currentClique[j] != CliqueUnknown)
                        {
                            if (localCompat[currentClique[j]][i] == 0)
                            {
                                compatibility = CliqueFalse;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (compatibility == CliqueTrue)
                    {
                        setY[index] = i;
                        index++;
                    }
                }
            }
            return index;
        }
        private void printSetY(int[] setY)
        {
            var index = 0;
            print(" setY = {");
            while (setY[index] != CliqueUnknown)
            {
                print(" %d ", setY[index]);
                index++;
            }
            print("}\n");
        }
        private void formSetY1(int nodesize, int[] setY, int[] setY1, int[][] setsIy)
        {
            var cards = new int[nodesize];

            for (var i = 0; i < nodesize; i++)
            {
                setY1[i] = CliqueUnknown;
                cards[i] = 0;
            }

            /* Get the cardinalities of  intersection(I_y, setY) 
               for each y in I_y */
            for (var i = 0; i < nodesize; i++) /* for each y in I_y */
            {
                if (setY[i] != CliqueUnknown)
                {
                    var currY = setY[i];
                    for (var j = 0; j < nodesize; j++) /* for each node in I_y of curr_y*/
                    {
                        if (setsIy[currY][j] != CliqueUnknown)
                        {
                            for (var k = 0; k < nodesize; k++) /* for each node in set_Y */
                            {
                                if (setY[k] != CliqueUnknown)
                                {
                                    if (setsIy[i][j] == setY[k])
                                    {
                                        cards[i]++;
                                    }
                                }
                                else
                                {
                                    break; /* end of set_Y */
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            var minVal = cards[0];
            for (var i = 0; i < nodesize; i++)
            {
                if (setY[i] != CliqueUnknown)
                {
                    if (cards[i] < minVal)
                    {
                        minVal = cards[i];
                    }
                }
            }

#if DEBUG
            print(" min_val = %d ", minVal);
#endif

            var currIndex = 0;
            for (var i = 0; i < nodesize; i++)
            {
                if (cards[i] == minVal)
                {
                    setY1[currIndex] = setY[i];
                    currIndex++;
                }
            }

#if DEBUG
            print(" Set Y1 = { ");
            for (var i = 0; i < nodesize; i++)
            {
                if (setY1[i] != CliqueUnknown)
                {
                    print(" %d ", setY1[i]);
                }
            }
            print(" }\n");
#endif
        }
        private void formSetY2(int nodesize, int[] setY2, int[] setY1, int[] sizesOfSetsIy)
        {
            var maxVal = CliqueUnknown;

            for (var i = 0; i < nodesize; i++)
            {
                if (setY1[i] != CliqueUnknown)
                {
                    if (sizesOfSetsIy[setY1[i]] > maxVal)
                    {
                        maxVal = sizesOfSetsIy[setY1[i]];
                    }
                }
                else
                {
                    break;
                }
            }

            var currIndex = 0;
            for (var i = 0; i < nodesize; i++)
            {
                if (setY1[i] != CliqueUnknown)
                {
                    if (sizesOfSetsIy[setY1[i]] == maxVal)
                    {
                        setY2[currIndex] = setY1[i];
                        currIndex++;
                    }
                }
                else
                {
                    break;
                }
            }

#if DEBUG
            print(" curr_index = %d   max_val = %d ", currIndex, maxVal);
            print(" Set Y2 = { ");
            for (var i = 0; i < nodesize; i++)
            {
                if (setY2[i] != CliqueUnknown)
                {
                    print(" %d ", setY2[i]);
                }
                else
                {
                    break;
                }
            }
            print(" }\n");
#endif
        }
        private int pickANodeToMerge(int[] setY, int[][] localCompat, int[] nodeSet, int nodesize)
        {
            var newNode = CliqueUnknown;
            /* dynamically allocate memory for sets_I_y array */

            var setsIy = new int[nodesize][];

            for (var i = 0; i < nodesize; i++)
            {
                setsIy[i] = new int[nodesize];
            }

            var currIndexes = new int[nodesize];
            var sizesOfSetsIy = new int[nodesize];

            for (var i = 0; i < nodesize; i++)
            {
                currIndexes[i] = 0;
                sizesOfSetsIy[i] = 0;
                for (var j = 0; j < nodesize; j++)
                {
                    setsIy[i][j] = CliqueUnknown;
                }
            }

            /* form I_y sets */

            for (var i = 0; i < nodesize; i++)
            {
                if (setY[i] != CliqueUnknown) /* for each y in Y do */
                {
                    for (var j = 0; j < nodesize; j++)
                    {
                        if (nodeSet[j] != CliqueUnknown)
                        {
                            /* if this node is still in set N */
                            if (localCompat[setY[i]][j] != 1)
                            {
                                setsIy[setY[i]][currIndexes[setY[i]]] = j;
                                currIndexes[setY[i]]++;
                            }
                        }
                    }
                }
                else
                {
                    break; /* end of setY */
                }
            }

            for (var i = 0; i < nodesize; i++)
            {
                if (setY[i] != CliqueUnknown) /* for each y in Y do */
                {
                    var currNodeInSetY = setY[i];

                    /* copy curr index into sizes */
                    sizesOfSetsIy[currNodeInSetY] = currIndexes[currNodeInSetY];

                    /* print all I_y sets */
#if DEBUG
                    print(" i= %d  nodeno= %d, curr_index = %d  ", i, currNodeInSetY, currIndexes[currNodeInSetY]);

                    printSetY(setsIy[currNodeInSetY]);
#endif
                }
            }

            /* form set_Y1 */
            var setY1 = new int[nodesize];
            for (var i = 0; i < nodesize; i++)
            {
                setY1[i] = CliqueUnknown;
            }
            formSetY1(nodesize, setY, setY1, setsIy);

            /* form set_Y2 */
            var setY2 = new int[nodesize];
            for (var i = 0; i < nodesize; i++)
            {
                setY2[i] = CliqueUnknown;
            }
            formSetY2(nodesize, setY2, setY1, sizesOfSetsIy);

            if (setY2[0] != CliqueUnknown)
            {
                newNode = setY2[0];
            }

            return newNode;
        }
        private void initCliqueSet()
        {
            print("\n Initializing the clique set..");
            for (var i = 0; i < Maxcliques; i++)
            {
                CliqueSet[i].Size = Unknown;
                for (var j = 0; j < Maxcliques; j++)
                {
                    CliqueSet[i].Members[j] = Unknown;
                }
            }
            print("..Done.\n");
        }
        private void printCliqueSet()
        {
            print("\n Clique Set: \n");

            for (var i = 0; i < Maxcliques; i++)
            {
                if (CliqueSet[i].Size == Unknown)
                {
                    break;
                }

                print("\tClique #%d (size = %d) = { ", i, CliqueSet[i].Size);

                for (var j = 0; j < Maxcliques; j++)
                {
                    if (CliqueSet[i].Members[j] != Unknown)
                    {
                        print(" %d ", CliqueSet[i].Members[j]);
                    }
                    else
                    {
                        break;
                    }
                }
                print(" }\n");
            }
            print("\n");
        }
        public void cliquePartition(int[][] compat, int nodesize)
        {
            print("\n");
            print("**************************************\n");
            print(" *       Clique Partitioner         *\n");
            print("**************************************\n");
            print("\nEntering Clique Partitioner.. \n");

            inputSanityCheck(compat, nodesize);

            /* dynamically allocate memory for local copy */

            var localCompat = new int[nodesize][];

            for (var i = 0; i < nodesize; i++)
            {
                localCompat[i] = new int[nodesize];
            }

            makeALocalCopy(localCompat, compat, nodesize);

            print(" You entered the compatibility array: \n");
            for (var i = 0; i < nodesize; i++)
            {
                print("\t");
                for (var j = 0; j < nodesize; j++)
                {
                    print("%d ", localCompat[i][j]);
                }
                print("\n");
            }

            initCliqueSet();

            /* allocate memory for current clique & initialize to unknown values*/
            /* - current_clique has the indices of nodes that are compatible with each other*/
            /* - A node i is in node_set if node_set[i] = i */

            var currentClique = new int[nodesize];
            var nodeSet = new int[nodesize];
            var setY = new int[nodesize];

            for (var i = 0; i < nodesize; i++)
            {
                currentClique[i] = CliqueUnknown;
                nodeSet[i] = i;
                setY[i] = CliqueUnknown;
            }

            var sizeN = nodesize;
            var currIndex = 0; /* reset the index to start for current clique */

            while (sizeN > 0) /* i.e still cliques to be formed */
            {
#if DEBUG
                print("=====================================================\n");
                print(" size_N = %d  node_set = { ", sizeN);
                for (var i = 0; i < nodesize; i++)
                {
                    print(" %d ", nodeSet[i]);
                }
                print(" }\n");
#endif

                if (currentClique[0] == CliqueUnknown) /* new clique formation */
                {
                    var nodeX = selectNewNode(localCompat, nodesize, nodeSet);
#if DEBUG
                    print(" Node x = %d \n", nodeX); /* first node in the clique */
#endif
                    currentClique[currIndex] = nodeX;
                    nodeSet[nodeX] = CliqueUnknown; /* remove node_x from N i.e node_set */
                    currIndex++;
                }

                var setYCardinality = formSetY(setY, currentClique, localCompat, nodesize, nodeSet);
#if DEBUG
                printSetY(setY);
#endif
                /* print (" Set Y cardinality = %d \n", setY_cardinality);*/

                if (setYCardinality == 0)
                /* No possible nodes for merger; 
                   declare current_cliqueas complete */
                {
                    /* copy the current clique into central datastructure */
                    var cliqueIndex = 0;
                    while (CliqueSet[cliqueIndex].Size != Unknown)
                    {
                        cliqueIndex++;
                    }

                    CliqueSet[cliqueIndex].Size = 0;

                    print(" A clique is found!! Clique = { ");
                    for (var i = 0; i < nodesize; i++)
                    {
                        if (currentClique[i] != CliqueUnknown)
                        {
                            CliqueSet[cliqueIndex].Members[i] = currentClique[i];

                            print(" %d ", currentClique[i]);
                            nodeSet[currentClique[i]] = CliqueUnknown; /* remove this node from the node list */
                            currentClique[i] = CliqueUnknown;
                            sizeN--;
                            CliqueSet[cliqueIndex].Size++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    print(" }\n");
                    currIndex = 0; /* reset the curr_index for the next clique */
                }
                else
                {
                    var nodeY = pickANodeToMerge(setY, localCompat, nodeSet, nodesize);
                    currentClique[currIndex] = nodeY;
                    nodeSet[nodeY] = CliqueUnknown;
#if DEBUG
                    print(" y (new node) = %d \n", nodeY);
#endif
                    currIndex++;
                }
            }
            outputSanityCheck(localCompat, compat);
            print("\n Final Clique Partitioning Results:\n");
            printCliqueSet();
            print("Exiting Clique Partitioner.. Bye.\n");
            print("**************************************\n\n");
        }
    }
}
