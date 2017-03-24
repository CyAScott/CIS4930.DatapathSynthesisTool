using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Synthesize.CliquePartition
{
    /// <summary>
    /// Ported from the clique_partition.c file in this repo.
    /// </summary>
    public static class CliqueHelper
    {
        // ReSharper disable once UnusedParameter.Local
        private static void assert(bool test)
        {
            if (!test)
            {
                throw new InvalidProgramException("The assert failed.");
            }
        }
        private static void print(bool enableConsolePrinting, string text, params int[] numbers)
        {
            if (enableConsolePrinting)
            {
                var index = 0;
                text = Regex.Replace(text, "%d", match => index < numbers.Length ? numbers[index++].ToString() : "%d");
                Console.Write(text);
            }
        }
        private static void exit(int exitCode)
        {
            throw new InvalidProgramException("The program should exit with code: " + exitCode);
        }
        
        public class Clique
        {
            public int[] Members = new int[Maxcliques];/* members of the clique */
            public int Size;/* number of members in the clique */
        }

        public const int CliqueFalse = 110;
        public const int CliqueTrue = 100;
        public const int CliqueUnknown = -12345;
        public const int Maxcliques = 200;
        public const int Unknown = -12345;

        private static int formSetY(int[] setY, int[] currentClique, int[][] localCompat, int[] nodeSet)
        {
            var index = 0;

            /* reset set_Y */
            for (var i = 0; i < localCompat.Length; i++)
            {
                setY[i] = CliqueUnknown;
            }

            for (var i = 0; i < localCompat.Length; i++)
            {
                var compatibility = CliqueTrue;
                if (nodeSet[i] != CliqueUnknown)
                {
                    for (var j = 0; j < localCompat.Length; j++)
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
        private static int getDegreeOfANode(int x, int[][] localCompat, int[] nodeSet)
        {
            var nodeDegree = 0;
            for (var j = 0; j < localCompat.Length; j++) /* compute node degrees */
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
        private static int pickANodeToMerge(bool enableConsolePrinting, int[] setY, int[][] localCompat, int[] nodeSet)
        {
            var newNode = CliqueUnknown;
            /* dynamically allocate memory for sets_I_y array */

            var setsIy = new int[localCompat.Length][];

            for (var i = 0; i < localCompat.Length; i++)
            {
                setsIy[i] = new int[localCompat.Length];
            }

            var currIndexes = new int[localCompat.Length];
            var sizesOfSetsIy = new int[localCompat.Length];

            for (var i = 0; i < localCompat.Length; i++)
            {
                currIndexes[i] = 0;
                sizesOfSetsIy[i] = 0;
                for (var j = 0; j < localCompat.Length; j++)
                {
                    setsIy[i][j] = CliqueUnknown;
                }
            }

            /* form I_y sets */

            for (var i = 0; i < localCompat.Length; i++)
            {
                if (setY[i] != CliqueUnknown) /* for each y in Y do */
                {
                    for (var j = 0; j < localCompat.Length; j++)
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

            for (var i = 0; i < localCompat.Length; i++)
            {
                if (setY[i] != CliqueUnknown) /* for each y in Y do */
                {
                    var currNodeInSetY = setY[i];

                    /* copy curr index into sizes */
                    sizesOfSetsIy[currNodeInSetY] = currIndexes[currNodeInSetY];

                    /* print all I_y sets */
#if DEBUG
                    print(enableConsolePrinting, " i= %d  nodeno= %d, curr_index = %d  ", i, currNodeInSetY, currIndexes[currNodeInSetY]);

                    printSetY(enableConsolePrinting, setsIy[currNodeInSetY]);
#endif
                }
            }

            /* form set_Y1 */
            var setY1 = new int[localCompat.Length];
            for (var i = 0; i < localCompat.Length; i++)
            {
                setY1[i] = CliqueUnknown;
            }
            formSetY1(enableConsolePrinting, setY, setY1, setsIy);

            /* form set_Y2 */
            var setY2 = new int[localCompat.Length];
            for (var i = 0; i < localCompat.Length; i++)
            {
                setY2[i] = CliqueUnknown;
            }
            formSetY2(enableConsolePrinting, setY2, setY1, sizesOfSetsIy);

            if (setY2[0] != CliqueUnknown)
            {
                newNode = setY2[0];
            }

            return newNode;
        }
        private static int selectNewNode(bool enableConsolePrinting, int[][] localCompat, int[] nodeSet)
        {
            /*    if a node with priority, then pick that node 
             *      else a node with highest degree
             *        if multiple nodes then pick a node
             *           with highest neighbor wt
             *             if multiple pick one randomly.   
             */
            int index;
            var degrees = new int[localCompat.Length][];
            var maxNode = CliqueUnknown;

            for (var i = 0; i < localCompat.Length; i++) /* initialize the degrees matrix */
            {
                degrees[i] = new int[localCompat.Length];
                for (var j = 0; j < localCompat.Length; j++)
                {
                    /* j dimension = node with degree=i */
                    degrees[i][j] = CliqueUnknown;
                }
            }

            var currMaxDegree = 0;
            for (var i = 0; i < localCompat.Length; i++) /* for each node do */
            {
                if (nodeSet[i] != CliqueUnknown) /* if the node is still in N */
                {
                    var currNodeDegree = getDegreeOfANode(i, localCompat, nodeSet);

#if DEBUG
                    print(enableConsolePrinting, " node=%d curr_node_degree = %d \n", i, currNodeDegree);
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

                for (index = 0; index < localCompat.Length; index++)
                /* go through all nodes with curr_max_degree*/
                {
                    if (degrees[currMaxDegree][index] != CliqueUnknown)
                    /* not end of list of the nodes with curr_max_degree */
                    {
                        var currNeighborsWt = 0;
                        var currNode = degrees[currMaxDegree][index];

                        /* get cumulative neighbor weight for this node */
                        currNeighborsWt += getDegreeOfANode(currNode, localCompat, nodeSet);
#if DEBUG
                        print(enableConsolePrinting, "curr_node = %d curr_neighbors_wt=%d\n", currNode, currNeighborsWt);
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
            print(enableConsolePrinting, " curr_max_degree = %d max_node= %d\n", currMaxDegree, maxNode);
#endif

            return maxNode;
        }
        private static void formSetY1(bool enableConsolePrinting, int[] setY, int[] setY1, int[][] setsIy)
        {
            var cards = new int[setY.Length];

            for (var i = 0; i < setY.Length; i++)
            {
                setY1[i] = CliqueUnknown;
                cards[i] = 0;
            }

            /* Get the cardinalities of  intersection(I_y, setY) 
               for each y in I_y */
            for (var i = 0; i < setY.Length; i++) /* for each y in I_y */
            {
                if (setY[i] != CliqueUnknown)
                {
                    var currY = setY[i];
                    for (var j = 0; j < setY.Length; j++) /* for each node in I_y of curr_y*/
                    {
                        if (setsIy[currY][j] != CliqueUnknown)
                        {
                            foreach (var t in setY)
                            {
                                if (t != CliqueUnknown)
                                {
                                    if (setsIy[i][j] == t)
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
            for (var i = 0; i < setY.Length; i++)
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
            print(enableConsolePrinting, " min_val = %d ", minVal);
#endif

            var currIndex = 0;
            for (var i = 0; i < setY.Length; i++)
            {
                if (cards[i] == minVal)
                {
                    setY1[currIndex] = setY[i];
                    currIndex++;
                }
            }

#if DEBUG
            print(enableConsolePrinting, " Set Y1 = { ");
            for (var i = 0; i < setY.Length; i++)
            {
                if (setY1[i] != CliqueUnknown)
                {
                    print(enableConsolePrinting, " %d ", setY1[i]);
                }
            }
            print(enableConsolePrinting, " }\n");
#endif
        }
        private static void formSetY2(bool enableConsolePrinting, int[] setY2, int[] setY1, int[] sizesOfSetsIy)
        {
            var maxVal = CliqueUnknown;

            for (var i = 0; i < setY2.Length; i++)
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
            for (var i = 0; i < setY2.Length; i++)
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
            print(enableConsolePrinting, " curr_index = %d   max_val = %d ", currIndex, maxVal);
            print(enableConsolePrinting, " Set Y2 = { ");
            foreach (var t in setY2)
            {
                if (t != CliqueUnknown)
                {
                    print(enableConsolePrinting, " %d ", t);
                }
                else
                {
                    break;
                }
            }
            print(enableConsolePrinting, " }\n");
#endif
        }
        private static void initCliqueSet(Clique[] cliqueSet, bool enableConsolePrinting)
        {
            print(enableConsolePrinting, "\n Initializing the clique set..");
            for (var i = 0; i < Maxcliques; i++)
            {
                cliqueSet[i].Size = Unknown;
                for (var j = 0; j < Maxcliques; j++)
                {
                    cliqueSet[i].Members[j] = Unknown;
                }
            }
            print(enableConsolePrinting, "..Done.\n");
        }
        private static void inputSanityCheck(bool enableConsolePrinting, int[][] compat)
        {
            /* Verifies whether the compat array passed is valid array
             *  (1) Is each array entry =0 or 1?
             *  (2) Is the matrix symmetric
             * Note that diagonal entries can be either 0 or 1.
             */

            print(enableConsolePrinting, " Checking the sanity of the input..");

            for (var i = 0; i < compat.Length; i++)
            {
                for (var j = 0; j < compat.Length; j++)
                {
                    if ((compat[i][j] != 1) && (compat[i][j] != 0))
                    {
                        print(enableConsolePrinting, " %d \n", compat[i][j]);
                        print(enableConsolePrinting, "The value of an array element is other than 1 or 0. Aborting..\n");
                        exit(0);
                    }
                    if (compat[i][j] != compat[j][i])
                    {
                        print(enableConsolePrinting, "The compatibility array is NOT symmetric at (%d,%d) and (%d,%d)! Aborting..\n ", i, j, j, i);
                        exit(0);
                    }
                    print(enableConsolePrinting, ".");
                }
            }

            print(enableConsolePrinting, "Done.\n");
        }
        private static void outputSanityCheck(Clique[] cliqueSet, bool enableConsolePrinting, int[][] localCompat, int[][] compat)
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
            print(enableConsolePrinting, "\n Verifying the results of the clique partitioning algorithm..");
            for (var i = 0; i < Maxcliques; i++)
            {
                if (cliqueSet[i].Size != Unknown)
                {
                    assert(cliqueSet[i].Size > 0);
                    for (var j = 0; j < cliqueSet[i].Size; j++)
                    {
                        for (var k = 0; k < cliqueSet[i].Size; k++)
                        {
                            if (j != k)
                            {
                                var member1 = cliqueSet[i].Members[j];
                                var member2 = cliqueSet[i].Members[k];

                                assert(compat[member1][member2] == 1);
                                assert(compat[member2][member1] == 1);
                                assert(localCompat[member2][member1] == 1);
                                assert(localCompat[member2][member1] == 1);
                                print(enableConsolePrinting, ".");
                            }
                        }
                    }
                }
            }
            print(enableConsolePrinting, "..Done.\n");
        }
        private static void makeALocalCopy(int[][] localCompat, int[][] compat)
        {
            for (var i = 0; i < compat.Length; i++)
            {
                for (var j = 0; j < compat.Length; j++)
                {
                    localCompat[i][j] = compat[i][j];
                }
            }
        }
        private static void printCliqueSet(Clique[] cliqueSet, bool enableConsolePrinting)
        {
            print(enableConsolePrinting, "\n Clique Set: \n");

            for (var i = 0; i < Maxcliques; i++)
            {
                if (cliqueSet[i].Size == Unknown)
                {
                    break;
                }

                print(enableConsolePrinting, "\tClique #%d (size = %d) = { ", i, cliqueSet[i].Size);

                for (var j = 0; j < Maxcliques; j++)
                {
                    if (cliqueSet[i].Members[j] != Unknown)
                    {
                        print(enableConsolePrinting, " %d ", cliqueSet[i].Members[j]);
                    }
                    else
                    {
                        break;
                    }
                }
                print(enableConsolePrinting, " }\n");
            }
            print(enableConsolePrinting, "\n");
        }
        private static void printSetY(bool enableConsolePrinting, int[] setY)
        {
            var index = 0;
            print(enableConsolePrinting, " setY = {");
            while (setY[index] != CliqueUnknown)
            {
                print(enableConsolePrinting, " %d ", setY[index]);
                index++;
            }
            print(enableConsolePrinting, "}\n");
        }
        
        /// <summary>
        /// Performs a clique partitioning algorithm on a matrix.
        /// </summary>
        public static Clique[] CliquePartition(int[][] compat, bool enableConsolePrinting = true)
        {
            print(enableConsolePrinting, "\n");
            print(enableConsolePrinting, "**************************************\n");
            print(enableConsolePrinting, " *       Clique Partitioner         *\n");
            print(enableConsolePrinting, "**************************************\n");
            print(enableConsolePrinting, "\nEntering Clique Partitioner.. \n");

            var cliqueSet = Enumerable
                .Range(0, Maxcliques)
                .Select(index => new Clique())
                .ToArray();

            inputSanityCheck(enableConsolePrinting, compat);

            /* dynamically allocate memory for local copy */

            var localCompat = new int[compat.Length][];

            for (var i = 0; i < compat.Length; i++)
            {
                localCompat[i] = new int[compat.Length];
            }

            makeALocalCopy(localCompat, compat);

            print(enableConsolePrinting, " You entered the compatibility array: \n");
            for (var i = 0; i < compat.Length; i++)
            {
                print(enableConsolePrinting, "\t");
                for (var j = 0; j < compat.Length; j++)
                {
                    print(enableConsolePrinting, "%d ", localCompat[i][j]);
                }
                print(enableConsolePrinting, "\n");
            }

            initCliqueSet(cliqueSet, enableConsolePrinting);

            /* allocate memory for current clique & initialize to unknown values*/
            /* - current_clique has the indices of nodes that are compatible with each other*/
            /* - A node i is in node_set if node_set[i] = i */

            var currentClique = new int[compat.Length];
            var nodeSet = new int[compat.Length];
            var setY = new int[compat.Length];

            for (var i = 0; i < compat.Length; i++)
            {
                currentClique[i] = CliqueUnknown;
                nodeSet[i] = i;
                setY[i] = CliqueUnknown;
            }

            var sizeN = compat.Length;
            var currIndex = 0; /* reset the index to start for current clique */

            while (sizeN > 0) /* i.e still cliques to be formed */
            {
#if DEBUG
                print(enableConsolePrinting, "=====================================================\n");
                print(enableConsolePrinting, " size_N = %d  node_set = { ", sizeN);
                for (var i = 0; i < compat.Length; i++)
                {
                    print(enableConsolePrinting, " %d ", nodeSet[i]);
                }
                print(enableConsolePrinting, " }\n");
#endif

                if (currentClique[0] == CliqueUnknown) /* new clique formation */
                {
                    var nodeX = selectNewNode(enableConsolePrinting, localCompat, nodeSet);
#if DEBUG
                    print(enableConsolePrinting, " Node x = %d \n", nodeX); /* first node in the clique */
#endif
                    currentClique[currIndex] = nodeX;
                    nodeSet[nodeX] = CliqueUnknown; /* remove node_x from N i.e node_set */
                    currIndex++;
                }

                var setYCardinality = formSetY(setY, currentClique, localCompat, nodeSet);
#if DEBUG
                printSetY(enableConsolePrinting, setY);
#endif
                /* print (" Set Y cardinality = %d \n", setY_cardinality);*/

                if (setYCardinality == 0)
                /* No possible nodes for merger; 
                   declare current_cliqueas complete */
                {
                    /* copy the current clique into central datastructure */
                    var cliqueIndex = 0;
                    while (cliqueSet[cliqueIndex].Size != Unknown)
                    {
                        cliqueIndex++;
                    }

                    cliqueSet[cliqueIndex].Size = 0;

                    print(enableConsolePrinting, " A clique is found!! Clique = { ");
                    for (var i = 0; i < compat.Length; i++)
                    {
                        if (currentClique[i] != CliqueUnknown)
                        {
                            cliqueSet[cliqueIndex].Members[i] = currentClique[i];

                            print(enableConsolePrinting, " %d ", currentClique[i]);
                            nodeSet[currentClique[i]] = CliqueUnknown; /* remove this node from the node list */
                            currentClique[i] = CliqueUnknown;
                            sizeN--;
                            cliqueSet[cliqueIndex].Size++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    print(enableConsolePrinting, " }\n");
                    currIndex = 0; /* reset the curr_index for the next clique */
                }
                else
                {
                    var nodeY = pickANodeToMerge(enableConsolePrinting, setY, localCompat, nodeSet);
                    currentClique[currIndex] = nodeY;
                    nodeSet[nodeY] = CliqueUnknown;
#if DEBUG
                    print(enableConsolePrinting, " y (new node) = %d \n", nodeY);
#endif
                    currIndex++;
                }
            }
            outputSanityCheck(cliqueSet, enableConsolePrinting, localCompat, compat);
            print(enableConsolePrinting, "\n Final Clique Partitioning Results:\n");
            printCliqueSet(cliqueSet, enableConsolePrinting);
            print(enableConsolePrinting, "Exiting Clique Partitioner.. Bye.\n");
            print(enableConsolePrinting, "**************************************\n\n");

            return cliqueSet;
        }
    }
}
