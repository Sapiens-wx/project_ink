using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Bounds bounds;
    [SerializeField] Vector2 gridSize;
    public LayerMask layerMask;
    //debug params
    public Vector2 _start, _to;
    public bool showGroundNodes,showAirNodes;

    List<Node> path;

    public static PathFinder inst;
    Node[,] grid_g;
    private List<Node> nodes_g;
    public List<Node> Nodes_g{
        get{
            if(nodes_g!=null) return nodes_g;
            nodes_g=new List<Node>();
            for(int i=0;i<grid_g.GetLength(0);++i){
                for(int j=0;j<grid_g.GetLength(1);++j){
                    if(grid_g[i,j]!=null)
                        nodes_g.Add(grid_g[i,j]);
                }
            }
            return nodes_g;
        }
    }
    private List<Node> nodes_a;
    public List<Node> Nodes_a{
        get{
            if(nodes_a!=null) return nodes_a;
            nodes_a=new List<Node>();
            for(int i=0;i<grid_a.GetLength(0);++i){
                for(int j=0;j<grid_a.GetLength(1);++j){
                    if(grid_a[i,j]!=null)
                        nodes_a.Add(grid_a[i,j]);
                }
            }
            return nodes_a;
        }
    }
    public Vector2 GridSize{
        get=>gridSize;
    }
    void Awake(){
        inst=this;
        CreateNodes_g();
    }
    [ContextMenu("find path_g")]
    void DebugFunc(){
        path=FindPath_g(_start, _to, 0);
    }
    [ContextMenu("find path_a")]
    void DebugFunc_a(){
        path=FindPath_a(_start, _to);
    }
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        int w=(int)((bounds.size.x+gridSize.x-1)/gridSize.x);
        int h=(int)((bounds.size.y+gridSize.y-1)/gridSize.y);
        if(showGroundNodes){
            if(grid_g!=null){
                Gizmos.color=Color.green;
                for(int i=0;i<w;++i){
                    for(int j=0;j<h;++j){
                        if(grid_g[i,j]!=null){
                            Gizmos.DrawWireSphere(grid_g[i,j].worldPos,.2f);
                            foreach(Node n in grid_g[i,j].nodes){
                                Gizmos.DrawLine(n.worldPos, grid_g[i,j].worldPos);
                            }
                        }
                    }
                }
            }
        }
        if(showAirNodes){
            if(grid_a!=null){
                Gizmos.color=Color.green;
                for(int i=0;i<w;++i){
                    for(int j=0;j<h;++j){
                        if(grid_a[i,j]!=null){
                            Gizmos.DrawWireSphere(grid_a[i,j].worldPos,.2f);
                            foreach(Node n in grid_a[i,j].nodes){
                                Gizmos.DrawLine(n.worldPos, grid_a[i,j].worldPos);
                            }
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
    public Node GetNeareastNode_g(Vector2 pos, float verticalOffset){
        pos.y+=verticalOffset;
        List<Node> candidates=Nodes_g;
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
    [ContextMenu("create nodes_g")]
    public void CreateNodes_g(){
        nodes_g=null; //clear the array that stores all the previous nodes
        int jumpXmin=2, jumpXmax=3, jumpY=4;
        int jumpDownX=3;

        int w=(int)((bounds.size.x+gridSize.x-1)/gridSize.x);
        int h=(int)((bounds.size.y+gridSize.y-1)/gridSize.y);
        grid_g=new Node[w,h];
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
                    grid_g[i,j]=new Node(new Vector2Int(i,j));
                    grid_g[i,j].worldPos=pos;
                }
            }
        }

        //eliminate nodes
        bool[,] isNodeConnected=new bool[w,h]; //is a node connected by other nodes who have different y pos than the given node.
        for(int j=0;j<h;++j){
            for(int i=0;i<w;++i){
                if(grid_g[i,j]!=null){
                    //---connect horizontal nodes--- [only connect the nodes on its right]
                    bool hasRightNode=false;
                    //add its right node
                    if(i<w-1&&grid_g[i+1,j]!=null){
                        grid_g[i,j].ConnectBothNode(grid_g[i+1,j]);
                        hasRightNode=true;
                    }
                    if(hasRightNode&&!isNodeConnected[i,j]&&grid_g[i,j].nodes.Count>=2){ //if has node on its right and has 2 nodes connected(not sure whether the other node is its left neighbor)
                        //if(grid[i,j].nodes[0].gridPos.y==grid[i,j].gridPos.y){//check it 
                        //remove the nodes. reconnect the two neighbor nodes
                        grid_g[i,j].nodes[^1].RemoveConnectedNode(grid_g[i,j]);
                        grid_g[i,j].nodes[^1].nodes.Add(grid_g[i,j].nodes[^2]);
                        grid_g[i,j].nodes[^2].RemoveConnectedNode(grid_g[i,j]);
                        grid_g[i,j].nodes[^2].nodes.Add(grid_g[i,j].nodes[^1]);
                        grid_g[i,j]=null;
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
                                else if(grid_g[leftx,y]!=null){
                                    isNodeConnected[leftx,y]=true;
                                    grid_g[i,j].ConnectNode(grid_g[leftx,y]);
                                    leftWallY=y+1;
                                }
                            }
                        }
                        if(rightx<w){
                            if(walls[rightx,j]) rightWallY=j;
                            for(int y=j+1;y<rightWallY;++y){
                                if(walls[rightx,y])
                                    rightWallY=y;
                                else if(grid_g[rightx,y]!=null){
                                    isNodeConnected[rightx,y]=true;
                                    grid_g[i,j].ConnectNode(grid_g[rightx,y]);
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
                            if(x-i>=jumpXmin && grid_g[x,y]!=null){ //can jump to this position
                                grid_g[i,j].ConnectBothNode(grid_g[x,y]);
                            }
                        }
                        //left
                        for(int x=i-1;x>=endX_left;--x){
                            if(walls[x,y]){
                                endX_left=x;
                                break;
                            }
                            if(i-x>=jumpXmin && grid_g[x,y]!=null){ //can jump to this position
                                grid_g[i,j].ConnectBothNode(grid_g[x,y]);
                            }
                        }
                    }
                    //if(grid[i,j].nodes.Count)
                }
            }
        }
    }
#region Air
    Node[,] grid_a;
    public Node GetNeareastNode_a(Vector2 pos){
        List<Node> candidates=Nodes_a;
        float totalMinDist=float.MaxValue;
        int totalDistSel=-1;
        //remove the points that are not in a horizontal line with the point
        for(int i=0;i<candidates.Count;++i){
            Vector2 dir=pos-candidates[i].worldPos;
            float dist=dir.x*dir.x+dir.y*dir.y;
            if(dist<totalMinDist){
                totalMinDist=dist;
                totalDistSel=i;
            }
        }
        if(totalDistSel==-1) return null;
        return candidates[totalDistSel];
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
    /// <summary>
    /// initialize nodes for pathfinding in the air
    /// </summary>
    [ContextMenu("create nodes_a")]
    public void CreateNodes_a(){
        int w=(int)((bounds.size.x+gridSize.x-1)/gridSize.x);
        int h=(int)((bounds.size.y+gridSize.y-1)/gridSize.y);
        grid_a=new Node[w,h];

        //add points ground
        Vector2 startPos=bounds.center;
        startPos.x+=gridSize.x/2-bounds.extents.x;
        startPos.y+=bounds.extents.y-gridSize.y/2;
        Vector2 pos=startPos;
        for(int i=0;i<w;++i){
            pos.x=startPos.x+i*gridSize.x;
            for(int j=0;j<h;++j){
                pos.y=startPos.y-j*gridSize.y;
                if(!Physics2D.OverlapPoint(pos, layerMask)){
                    grid_a[i,j]=new Node(new Vector2Int(i,j));
                    grid_a[i,j].worldPos=pos;
                }
            }
        }
        for(int i=0;i<w;++i){
            for(int j=0;j<h;++j){
                if(grid_a[i,j]==null) continue;
                int i1=i+1,j1=j+1,i_1=i-1;
                bool hasLeftNode=i_1>=0&&grid_a[i_1,j]!=null;
                bool hasRightNode=i1<w&&grid_a[i1,j]!=null;
                bool hasDownNode=j1<h&&grid_a[i,j1]!=null;
                if(hasRightNode) //right node
                    grid_a[i,j].ConnectBothNode(grid_a[i1,j]);
                if(hasDownNode){ //down node
                    grid_a[i,j].ConnectBothNode(grid_a[i,j1]);
                    if(hasLeftNode && grid_a[i_1,j1]!=null)
                        grid_a[i,j].ConnectBothNode(grid_a[i_1,j1]);
                    if(hasRightNode && grid_a[i1,j1]!=null)
                        grid_a[i,j].ConnectBothNode(grid_a[i1,j1]);
                }
            }
        }
    }
#endregion
#region path finding
    PathNode[,] hash;
    Node targetNode;
    HashHeap<PathNode> heap;
    public List<Node> FindPath_a(Vector2 start, Vector2 end){
        return FindPath_a(GetNeareastNode_a(start), GetNeareastNode_a(end));
    }
    public List<Node> FindPath_g(Vector2 start, Vector2 end, float yOffset){
        return FindPath_g(GetNeareastNode_g(start,yOffset), GetNeareastNode_g(end,yOffset));
    }
    public List<Node> FindPath_a(Node startNode, Node endNode){
        if(startNode==null || endNode==null)
            return null;
        targetNode=endNode;
        hash=new PathNode[grid_a.Length, grid_a.GetLength(1)];
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
    public List<Node> FindPath_g(Node startNode, Node endNode){
        if(startNode==null || endNode==null)
            return null;
        targetNode=endNode;
        hash=new PathNode[grid_g.Length, grid_g.GetLength(1)];
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
