﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Btree {
    public partial class Form1 : Form {
        /* List bug :
         * bug search kotak/string
         * bug search line
         * bug posisi line jika ordo > 3
         */
        Btree bt;
        int ordoSize, insertValue, deleteValue, searchValue;
        // untuk membantu visualisasi search
        bool onSearch;
        List<int> searchTraverseIndex;

        public Form1() {
            InitializeComponent();
            onSearch = false;
        }
        // SET
        private void BtnOrdo_Click(object sender, EventArgs e) {
            SetOrdo();
        }
        private void TbOrdo_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                SetOrdo();
        }
        private void SetOrdo() {
            ordoSize = int.Parse(tbOrdo.Text);
            bt = new Btree(ordoSize);
            tbOrdo.Text = "";
            panel1.Refresh();

            tbInsert.Enabled = true;
            btnInsert.Enabled = true;
            tbSearch.Enabled = true;
            btnSearch.Enabled = true;
            tbDelete.Enabled = true;
            btnDelete.Enabled = true;

            btnOrdo.Text = "Reset Tree";
        }
        // INSERT
        private void BtnInsert_Click(object sender, EventArgs e) {
            InsertKey();
        }
        private void TbInsert_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                InsertKey();
        }
        private void InsertKey() {
            insertValue = int.Parse(tbInsert.Text);
            bt.insert(ref bt.root, insertValue);
            Console.WriteLine("Insert " + tbInsert.Text);
            tbInsert.Text = "";
            panel1.Refresh();
        }
        // SEARCH
        private void BtnSearch_Click(object sender, EventArgs e) {
            SearchKey();
        }
        private void TbSearch_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                SearchKey();
        }
        private void SearchKey() {
            searchValue = int.Parse(tbSearch.Text);
            searchTraverseIndex = bt.getFindTraverseIndex(bt.root, searchValue); // bug return array != array on Form1
            if (searchTraverseIndex!=null) {
                for (int i = 0; i < searchTraverseIndex.Count(); i++) {
                    Console.WriteLine("search traverse index (result in form1) : " + searchTraverseIndex[i]);
                }
                onSearch = true;
                panel1.Refresh();
                onSearch = false;
            }
            tbSearch.Text = "";
        }
        // DELETE
        private void BtnDelete_Click(object sender, EventArgs e) {
            DeleteKey();
        }
        private void TbDelete_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                DeleteKey();
        }
        private void DeleteKey() {
            deleteValue = int.Parse(tbDelete.Text);
            bt.delete(ref bt.root, deleteValue);
            tbDelete.Text = "";
            panel1.Refresh();
        }
        // DRAW
        private void Panel1_Paint(object sender, PaintEventArgs e) {
            if (bt != null) {
                // CONST
                const int RECTANGLE_SIZE = 30, RECTANGLE_Y_DISTANCE = 20;// RECTANGLE_SIZE also used as RECTANGLE_X_DISTANCE
                // non-search object
                List<Rectangle> nodeRectangle = new List<Rectangle>();
                List<int> nodeKey = new List<int>();
                List<LinePosition> nodeLines = new List<LinePosition>();
                // search object
                List<Rectangle> nodeSearchRectangle = new List<Rectangle>();
                List<int> nodeSearchKey = new List<int>();
                List<LinePosition> nodeSearchLines = new List<LinePosition>();
                // utility for non-search object
                Pen p = new Pen(Color.Black);
                Brush b = new SolidBrush(Color.Black);
                Font f = new Font("Courier New", 10, FontStyle.Regular);
                // utility for non-search object
                Pen ps = new Pen(Color.Red);
                Brush bs = new SolidBrush(Color.Red);
                Font fs = new Font("Courier New", 10, FontStyle.Bold);
                // NODE property
                List<FakeBNode> fakeBNodes = bt.getFakeBNodes();
                // help set object at center
                int[] oldEndPosition = new int[bt.maxDepth + 1];
                int[] newStartPosition = new int[bt.maxDepth + 1];


                int lastDepth = 0;
                int[] keysCount = new int[bt.maxDepth + 1], nodeCount = new int[bt.maxDepth + 1], childCount = new int[bt.maxDepth + 1];
                // i,j,... = fakeBNodes
                // a,b,... = fakeBNodes.keys
                // x,y,... = fakeBNodes.child
                // m,n,... = fakeBNodes.traverseIndex
                for (int i = 0; i < fakeBNodes.Count; i++) {
                    // step 1. to determine key position
                    if (fakeBNodes[i].depth != lastDepth) {
                        lastDepth = fakeBNodes[i].depth;
                        nodeCount[lastDepth] = 0;
                        childCount[lastDepth] = fakeBNodes[i].childCount;
                        keysCount[lastDepth] = fakeBNodes[i].keysCount;
                    } else {
                        if (i != 0)
                            nodeCount[lastDepth]++;
                        childCount[lastDepth] += fakeBNodes[i].childCount;
                        keysCount[lastDepth] += fakeBNodes[i].keysCount;
                    }

                    // BUG PARENT NODE && starting traverse index
                    bool isSearchPath = true;
                    int selectedSearchTraverseIndex=-1;
                    if (onSearch && fakeBNodes[i].traverseIndex.Count() <= searchTraverseIndex.Count()) {
                        for (int m = 0; m < fakeBNodes[i].traverseIndex.Count(); m++) {
                            if (fakeBNodes[i].traverseIndex[m] != searchTraverseIndex[m]) {
                                isSearchPath = false;
                                break;
                            }
                        }
                        if(isSearchPath) {
                            selectedSearchTraverseIndex = fakeBNodes[i].traverseIndex[fakeBNodes[i].traverseIndex.Count()-1];
                        }
                    }else{
                        isSearchPath = false;
                    }

                    for (int a = 0; a < fakeBNodes[i].keysCount; a++) {
                        // step 2. set key/rectangle position
                        int x = (keysCount[lastDepth] - fakeBNodes[i].keysCount + a) * RECTANGLE_SIZE + nodeCount[lastDepth] * RECTANGLE_SIZE;
                        int y = lastDepth * (RECTANGLE_SIZE + RECTANGLE_Y_DISTANCE);
                        if(onSearch && isSearchPath && (a == selectedSearchTraverseIndex || a-1== selectedSearchTraverseIndex || fakeBNodes[i].keys[a] == searchValue)){
                            nodeSearchRectangle.Add(new Rectangle(x, y, RECTANGLE_SIZE, RECTANGLE_SIZE));
                            nodeSearchKey.Add(fakeBNodes[i].keys[a]);
                        } else{
                            nodeRectangle.Add(new Rectangle(x, y, RECTANGLE_SIZE, RECTANGLE_SIZE));
                            nodeKey.Add(fakeBNodes[i].keys[a]);
                        }
                    }
                }

                // step 3. prepare line
                int currentNodeIndex = -1, currentChildIndex = 0, currentChildCount = 0;
                int childNodeIndex = 0, childKeyCount = 0;
                lastDepth = 0;
                for (int i = 0; i < fakeBNodes.Count; i++) {
                    if (fakeBNodes[i].depth != lastDepth) {
                        currentNodeIndex = 0;
                        currentChildIndex = 0;
                        currentChildCount = fakeBNodes[i].childCount;
                        lastDepth = fakeBNodes[i].depth;
                    } else {
                        currentNodeIndex++;
                        currentChildCount += fakeBNodes[i].childCount;
                    }

                    if (fakeBNodes[i].depth < bt.maxDepth) {
                        for (int x = 0; x < fakeBNodes[i].childCount; x++) {
                            childNodeIndex = 0;
                            childKeyCount = 0;
                            for (int j = i + 1; j < fakeBNodes.Count; j++) {
                                if (fakeBNodes[i].depth + 1 == fakeBNodes[j].depth) {
                                    childKeyCount += fakeBNodes[j].keysCount;
                                    if (childNodeIndex == currentChildIndex) {
                                        /*
                                        Console.WriteLine("================================");
                                        Console.WriteLine("Node parent : " + i);
                                        Console.WriteLine("currentChildCount : " + currentChildCount);
                                        Console.WriteLine("fakeBNodes[i].childCount : " + fakeBNodes[i].childCount);
                                        Console.WriteLine("x : " + x);
                                        Console.WriteLine("currentNodeIndex : " + currentNodeIndex);
                                        */

                                        int x1 = (currentNodeIndex + x) * RECTANGLE_SIZE + currentNodeIndex * RECTANGLE_SIZE; // bug
                                        int y1 = lastDepth * (RECTANGLE_SIZE + RECTANGLE_Y_DISTANCE) + RECTANGLE_SIZE;
                                        int x2 = childNodeIndex * RECTANGLE_SIZE + (childKeyCount - fakeBNodes[j].keysCount) * RECTANGLE_SIZE;
                                        int y2 = y1 + RECTANGLE_Y_DISTANCE;
                                        nodeLines.Add(new LinePosition(x1, y1, x2, y2));
                                    }
                                    childNodeIndex++;
                                }
                            }
                            currentChildIndex++;
                        }
                    }
                }

                // step 4. move everything to center
                // 4a. get startX endX
                int currentDepth = 0, lastY = 0, lastX = 0;
                for (int i = 0; i < nodeRectangle.Count(); i++) {
                    // change depth
                    if (i == 0) {
                        lastY = nodeRectangle[i].Top;
                    } else if (nodeRectangle[i].Top != lastY) {
                        // add to startX,endX
                        oldEndPosition[currentDepth] = lastX;

                        // reset
                        currentDepth++;
                        lastY = nodeRectangle[i].Top;
                    } else if (i == nodeRectangle.Count() - 1) {
                        oldEndPosition[currentDepth] = nodeRectangle[i].Right;
                    }

                    // get
                    if (i == 0) {
                        lastX = nodeRectangle[i].Right;
                    } else {
                        lastX = nodeRectangle[i].Right;
                    }
                }
                // 4b. set added X
                for (int i = 0; i < bt.maxDepth + 1; i++) {
                    newStartPosition[i] = (panel1.Width - oldEndPosition[i]) / 2;
                }
                // 4c. execute rectangle + string
                currentDepth = 0;
                lastY = 0;
                for (int i = 0; i < nodeRectangle.Count(); i++) {
                    if (i == 0) {
                        lastY = nodeRectangle[i].Top;
                    } else if (nodeRectangle[i].Top != lastY) {
                        currentDepth++;
                        lastY = nodeRectangle[i].Top;
                    }
                    nodeRectangle[i] = new Rectangle(nodeRectangle[i].X + newStartPosition[currentDepth], nodeRectangle[i].Y, nodeRectangle[i].Width, nodeRectangle[i].Height); // = endX[currentDepth];
                }

                // 4c. execute line
                currentDepth = 0;
                lastY = 0;
                for (int i = 0; i < nodeLines.Count; i++) {
                    if (i == 0) {
                        lastY = nodeLines[i].y1;
                    } else if (nodeLines[i].y1 != lastY) {
                        currentDepth++;
                        lastY = nodeLines[i].y1;
                    }

                    nodeLines[i] = new LinePosition(nodeLines[i].x1 + newStartPosition[currentDepth], nodeLines[i].y1, nodeLines[i].x2 + newStartPosition[currentDepth + 1], nodeLines[i].y2);
                }

                // step 5. draw rectangle string from
                for (int i = 0; i < nodeRectangle.Count; i++) {
                    e.Graphics.DrawRectangle(p, nodeRectangle[i]);
                    e.Graphics.DrawString(nodeKey[i].ToString(), f, b, nodeRectangle[i]);
                }
                for (int i = 0; i < nodeSearchRectangle.Count; i++) {
                    e.Graphics.DrawRectangle(ps, nodeSearchRectangle[i]);
                    e.Graphics.DrawString(nodeSearchKey[i].ToString(), f, bs, nodeSearchRectangle[i]);
                }

                // step 6. draw line
                for (int m = 0; m < nodeLines.Count; m++) {
                    e.Graphics.DrawLine(p, nodeLines[m].x1, nodeLines[m].y1, nodeLines[m].x2, nodeLines[m].y2);
                }

                //Console.WriteLine("Inorder!");
                //bt.inorder(ref bt.root);
            }
        }
    }
}
