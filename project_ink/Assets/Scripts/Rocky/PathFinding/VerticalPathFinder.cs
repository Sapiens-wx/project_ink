using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class VerticalPathFinder : MonoBehaviour
{
    [SerializeField] Bounds bounds;
    [SerializeField] Vector2 gridSize;
    public LayerMask layerMask;
    public Vector2 _start, _to;
    List<Node> path;

    public static VerticalPathFinder inst;
    Node[,] grid;
    private List<Node> nodes;
    public List<Node> Nodes{
        get{
            if(nodes!=null) return nodes;
            nodes=new List<Node>();
            for(int i=0;i<grid.GetLength(0);++i){
                for(int j=0;j<grid.GetLength(1);++j){
                    if(grid[i,j]!=null)
                        nodes.Add(grid[i,j]);
                }
            }
            return nodes;
        }
    }
    public Vector2 GridSize{
        get=>gridSize;
    }
    void Awake(){
        inst=this;
        CreateNodes();
    }
    [ContextMenu("find path")]
    void DebugFunc(){
        path=FindPath(_start, _to, 0);
    }
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        int w=(int)(bounds.size.x/gridSize.x)+1;
        int h=(int)(bounds.size.y/gridSize.y)+1;
        if(grid!=null){
            Gizmos.color=Color.green;
            for(int i=0;i<w;++i){
                for(int j=0;j<h;++j){
                    if(grid[i,j]!=null){
                        Gizmos.DrawWireSphere(grid[i,j].worldPos,.2f);
                        foreach(Node n in grid[i,j].nodes){
                            Gizmos.DrawLine(n.worldPos, grid[i,j].worldPos);
                        }
                    }
                }
            }
        }
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(_start, .3f);
        Gizmos.DrawWireSphere(_to, .3f);
        if(path!=null){
            Gizmos.color=Color.blue;
            for(int i=path.Count-1;i>0;--i){
                Gizmos.DrawLine(path[i].worldPos, path[i-1].worldPos);
            }
        }
    }
    public Node GetNeareastNode(Vector2 pos, float verticalOffset){
        pos.y+=verticalOffset;
        List<Node> candidates=Nodes;
        float xDist=float.MaxValue;
        int sel=-1;
        float totalMinDist=float.MaxValue;
        int totalDistSel=-1;
        //remove the points that are not in a horizontal line with the point
        for(int i=0;i<candidates.Count;++i){
            float dist=Mathf.Abs(candidates[i].worldPos.x-pos.x);
            if(Mathf.Abs(candidates[i].worldPos.y-pos.y)<=gridSize.y && dist<xDist){
                xDist=dist;
                sel=i;
            } else{
                Vector2 tmp=candidates[i].worldPos-pos;
                float sqrDist=tmp.x*tmp.x+tmp.y*tmp.y;
                if(sqrDist<totalMinDist){
                    totalMinDist=sqrDist;
                    totalDistSel=i;
                }
            }
        }
        if(sel==-1){
            if(totalDistSel>=0) return candidates[totalDistSel]; //there is a nearest node, but not in the same horizontal line
            return null;
        }
        return candidates[sel];
        /*
        candidates.Sort((l,r)=>{
            Vector2 tmp=l.worldPos-pos;
            float distL=tmp.x*tmp.x+tmp.y*tmp.y;
            tmp=r.worldPos-pos;
            float distR=tmp.x*tmp.x+tmp.y*tmp.y;
            if(distL>distR) return 1;
            else if(distL<distR) return -1;
            return 0;
        });
        */
    }
    [ContextMenu("create nodes")]
    public void CreateNodes(){
        nodes=null; //clear the array that stores all the previous nodes
        int jumpXmin=2, jumpXmax=3, jumpY=4;
        int jumpDownX=3;

        int w=(int)(bounds.size.x/gridSize.x)+1;
        int h=(int)(bounds.size.y/gridSize.y)+1;
        grid=new Node[w,h];
        bool[,] walls=new bool[w,h];

        //add points above ground
        Vector2 startPos=bounds.center;
        startPos.x+=gridSize.x/2-bounds.extents.x;
        startPos.y+=bounds.extents.y-gridSize.y/2;
        Vector2 pos=startPos;
        for(int i=0;i<w;++i){
            pos.x=startPos.x+i*gridSize.x;
            //bottom most row
            int j=h-1;
            pos.y=startPos.y-j*gridSize.y;
            bool hasWallHere=Physics2D.OverlapPoint(pos, layerMask);
            walls[i,j]=hasWallHere;
            for(--j;j>=0;--j){
                pos.y=startPos.y-j*gridSize.y;
                walls[i,j]=Physics2D.OverlapPoint(pos, layerMask);
                bool down=walls[i,j+1];
                if(! walls[i,j] && down){
                    grid[i,j]=new Node(new Vector2Int(i,j));
                    grid[i,j].worldPos=pos;
                }
            }
        }

        //eliminate nodes
        bool[,] isNodeConnected=new bool[w,h]; //is a node connected by other nodes who have different y pos than the given node.
        for(int j=0;j<h;++j){
            for(int i=0;i<w;++i){
                if(grid[i,j]!=null){
                    //---connect horizontal nodes--- [only connect the nodes on its right]
                    bool hasRightNode=false;
                    //add its right node
                    if(i<w-1&&grid[i+1,j]!=null){
                        grid[i,j].ConnectBothNode(grid[i+1,j]);
                        hasRightNode=true;
                    }
                    if(hasRightNode&&!isNodeConnected[i,j]&&grid[i,j].nodes.Count>=2){ //if has node on its right and has 2 nodes connected(not sure whether the other node is its left neighbor)
                        //if(grid[i,j].nodes[0].gridPos.y==grid[i,j].gridPos.y){//check it 
                        //remove the nodes. reconnect the two neighbor nodes
                        grid[i,j].nodes[^1].RemoveConnectedNode(grid[i,j]);
                        grid[i,j].nodes[^1].nodes.Add(grid[i,j].nodes[^2]);
                        grid[i,j].nodes[^2].RemoveConnectedNode(grid[i,j]);
                        grid[i,j].nodes[^2].nodes.Add(grid[i,j].nodes[^1]);
                        grid[i,j]=null;
                        continue;
                        //}
                    }
                    //---horizontal jump---
                    //---jump down---
                    int leftWallY=h; //the topmost y pos that has wall.
                    int rightWallY=h;
                    for(int xOffset=1;xOffset<=jumpXmax;++xOffset){
                        int leftx=i-xOffset, rightx=i+xOffset;
                        if(leftx>=0){
                            //if there is a wall, then cannot jump down
                            if(walls[leftx,j]) leftWallY=j;
                            //left side
                            for(int y=j+1;y<leftWallY;++y){
                                if(walls[leftx,y])
                                    leftWallY=y;
                                else if(grid[leftx,y]!=null){
                                    isNodeConnected[leftx,y]=true;
                                    grid[i,j].ConnectNode(grid[leftx,y]);
                                    leftWallY=y+1;
                                }
                            }
                        }
                        if(rightx<w){
                            if(walls[rightx,j]) rightWallY=j;
                            for(int y=j+1;y<rightWallY;++y){
                                if(walls[rightx,y])
                                    rightWallY=y;
                                else if(grid[rightx,y]!=null){
                                    isNodeConnected[rightx,y]=true;
                                    grid[i,j].ConnectNode(grid[rightx,y]);
                                    rightWallY=y+1;
                                }
                            }
                        }
                    }
                    //---vertically---
                    int endX_right=Mathf.Min(w-1, i+jumpXmax);
                    int endX_left=Mathf.Max(0,i-jumpXmax);
                    //update endX_right and left first on its horizontal floor
                    //right
                    for(int x=i+1;x<=endX_right;++x){
                        if(walls[x,j]){
                            endX_right=x;
                            break;
                        }
                    }
                    //left
                    for(int x=i-1;x>=endX_left;--x){
                        if(walls[x,j]){
                            endX_left=x;
                            break;
                        }
                    }
                    //start the loop to check for jump
                    int ymax=Mathf.Max(0,j-jumpY);
                    for(int y=j-1;y>=ymax;--y){
                        if(walls[i,y]) break;
                        //right
                        for(int x=i+1;x<=endX_right;++x){
                            if(walls[x,y]){
                                endX_right=x;
                                break;
                            }
                            if(x-i>=jumpXmin && grid[x,y]!=null){ //can jump to this position
                                grid[i,j].ConnectBothNode(grid[x,y]);
                            }
                        }
                        //left
                        for(int x=i-1;x>=endX_left;--x){
                            if(walls[x,y]){
                                endX_left=x;
                                break;
                            }
                            if(i-x>=jumpXmin && grid[x,y]!=null){ //can jump to this position
                                grid[i,j].ConnectBothNode(grid[x,y]);
                            }
                        }
                    }
                    //if(grid[i,j].nodes.Count)
                }
            }
        }
    }
    #region path finding
    PathNode[,] hash;
    Node targetNode;
    HashHeap<PathNode> heap;
    public List<Node> FindPath(Vector2 start, Vector2 end, float yOffset){
        return FindPath(GetNeareastNode(start,yOffset), GetNeareastNode(end,yOffset));
    }
    public List<Node> FindPath(Node startNode, Node endNode){
        if(startNode==null || endNode==null)
            return null;
        targetNode=endNode;
        hash=new PathNode[grid.Length, grid.GetLength(1)];
        heap=new HashHeap<PathNode>(PathNode.Compare);

        PathNode cur=new PathNode(startNode);
        AddToOpen(cur);
        for(;heap.Count!=0 && heap.Front().node!=endNode;){
            cur=heap.Pop();
            //move to closed
            cur.isClosed=true;
            AddSurroundingNodes(cur);
        }
        if(heap.Front().node!=endNode)
            return null;
        List<Node> result=new List<Node>();
        cur=heap.Front();
        while(cur!=null){
            result.Add(cur.node);
            cur=cur.parent;
        }
        result.Reverse();
        return result;
    }
    private void AddToOpen(PathNode node){
        //Debug.Log($"add node {node.node.gridPos}");
        hash[node.node.gridPos.x, node.node.gridPos.y]=node;
        heap.Insert(node);
    }
    private void AddSurroundingNodes(PathNode pathNode){
        foreach(Node node in pathNode.node.nodes){
            PathNode originalPathNode=hash[node.gridPos.x,node.gridPos.y];
            if(originalPathNode!=null && originalPathNode.isClosed)
                continue;
            PathNode adjNode=new PathNode(node);
            adjNode.parent=pathNode;
            adjNode.Gcost=pathNode.gcost+pathNode.Dist(adjNode);
            adjNode.Hcost=adjNode.Dist(targetNode.gridPos);
            if(originalPathNode!=null && originalPathNode.gcost>adjNode.gcost){
                heap.Remove(originalPathNode);
                AddToOpen(adjNode);
            } else
                AddToOpen(adjNode);
        }
    }
    #endregion
    public class Node{
        public Vector2Int gridPos;
        public Vector2 worldPos;
        public List<Node> nodes;
        public Node(Vector2Int pos){
            nodes=new List<Node>();
            gridPos=pos;
        }
        public void RemoveConnectedNode(Node node){
            int idx=nodes.IndexOf(node);
            if(idx==-1)
                throw new System.Exception("cannot find the node in nodes");
            if(idx!=nodes.Count-1){
                nodes[idx]=nodes[^1];
            }
            nodes.RemoveAt(nodes.Count-1);
        }
        public void RemoveBothConnectedNode(Node node){
            RemoveConnectedNode(node);
            node.RemoveConnectedNode(this);
        }
        public void ConnectNode(Node node){
            nodes.Add(node);
        }
        public void ConnectBothNode(Node node){
            ConnectNode(node);
            if(!node.ContainsNode(this))
                node.ConnectNode(this);
        }
        public bool ContainsNode(Node node){
            return nodes.Contains(node);
        }
    }
    public class PathNode{
        public Node node;
        public PathNode parent;
        public bool isClosed;
        /// <summary>
        /// dist from start
        /// </summary>
        public int gcost;
        /// <summary>
        /// dist to dest
        /// </summary>
        public int hcost;
        public int totalCost;
        public int Gcost{
            set{
                gcost=value;
                totalCost=gcost+hcost;
            }
        }
        public int Hcost{
            set{
                hcost=value;
                totalCost=gcost+hcost;
            }
        }
        public PathNode(Node node){
            this.node=node;
            isClosed=false;
        }
        public static bool Compare(PathNode lhs, PathNode rhs){
            return lhs.totalCost<rhs.totalCost;
        }
        public static int Dist(Vector2Int from, Vector2Int to){
            to-=from;
            return to.x*to.x+to.y*to.y;
        }
        public static int Dist(PathNode from, PathNode to){
            return Dist(from.node.gridPos, to.node.gridPos);
        }
        public int Dist(PathNode to){
            return Dist(node.gridPos, to.node.gridPos);
        }
        public int Dist(Vector2Int to){
            return Dist(node.gridPos, to);
        }
    }
}
