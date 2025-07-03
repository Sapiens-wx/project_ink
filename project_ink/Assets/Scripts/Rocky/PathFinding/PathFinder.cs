using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] PathFindingConfig config;
    [SerializeField] Bounds bounds;
    [SerializeField] Vector2 gridSize;
    //debug params
    public Vector2 _start, _to;
    public bool showGroundNodes,showAirNodes;

    List<Node> path;

    public static PathFinder inst;
    Node[,] grid_g;
    bool[,] walls;
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
        CreateNodes_a();
    }
    [ContextMenu("find path_g")]
    void DebugFunc(){
        path=FindPath_g(_start, _to, 0);
    }
    [ContextMenu("find path_a")]
    void DebugFunc_a(){
        path=FindPath_a(_start, _to);
    }
    void DrawGrid(){
        Vector2 min=bounds.min, max=bounds.max;
        for(float x=min.x+gridSize.x;x<max.x;x+=gridSize.x){
            Gizmos.DrawLine(new Vector2(x,min.y),new Vector2(x,max.y));
        }
        for(float y=max.y;y>min.y;y-=gridSize.y){
            Gizmos.DrawLine(new Vector2(min.x,y),new Vector2(max.x,y));
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        DrawGrid();
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
        LayerMask layerMask=GameManager.inst.groundMixLayer;
        nodes_g=null; //clear the array that stores all the previous nodes
        //int jumpDownX=3;

        int w=(int)((bounds.size.x+gridSize.x-1)/gridSize.x);
        int h=(int)((bounds.size.y+gridSize.y-1)/gridSize.y);
        grid_g=new Node[w,h];
        walls=new bool[w,h];

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
                    //if hasRightnode && is not connected by nodes with different y pos && is connected by a left node,
                    //then this node can be eliminated
                    if(hasRightNode&&!isNodeConnected[i,j]&&grid_g[i,j].nodes.Count>=2){
                        grid_g[i,j].nodes[^1].RemoveConnectedNode(grid_g[i,j]);
                        grid_g[i,j].nodes[^1].ConnectNode(grid_g[i,j].nodes[^2]);
                        grid_g[i,j].nodes[^2].RemoveConnectedNode(grid_g[i,j]);
                        grid_g[i,j].nodes[^2].ConnectNode(grid_g[i,j].nodes[^1]);
                        grid_g[i,j]=null;
                        continue;
                    }
                    //---jump down---
                    int leftWallY=h; //the topmost y pos that has wall.
                    int rightWallY=h;
                    for(int xOffset=1;xOffset<=config.jumpXmax;++xOffset){
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
                                    leftWallY=y+1; //the y+1 pos must be a wall. so terminate the loop
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
                    int endX_right=Mathf.Min(w-1, i+config.jumpXmax);
                    int endX_left=Mathf.Max(0,i-config.jumpXmax);
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
                    int ymax=Mathf.Max(0,j-config.jumpY);
                    for(int y=j-1;y>=ymax;--y){
                        if(walls[i,y]) break;
                        //right
                        for(int x=i+1;x<=endX_right;++x){
                            if(walls[x,y]){
                                endX_right=x;
                                break;
                            }
                            if(x-i>=config.jumpXmin && grid_g[x,y]!=null){ //can jump to this position
                                grid_g[i,j].ConnectBothNode(grid_g[x,y]);
                            }
                        }
                        //left
                        for(int x=i-1;x>=endX_left;--x){
                            if(walls[x,y]){
                                endX_left=x;
                                break;
                            }
                            if(i-x>=config.jumpXmin && grid_g[x,y]!=null){ //can jump to this position
                                grid_g[i,j].ConnectBothNode(grid_g[x,y]);
                            }
                        }
                    }
                    //if(grid[i,j].nodes.Count)
                }
            }
        }
        for(int j=0;j<h;++j){
            for(int i=0;i<w;++i){
                if(grid_g[i,j]!=null){
                    //---horizontal jump---
                    if(i<w-1&&j<h-1&&!walls[i+1,j+1]){ //make sure the node is on the right edge of a platform
                        int hjumpMaxX=Mathf.Min(w,i+config.horizontalJumpXMax);
                        int x=i+1;
                        for(int y=Mathf.Max(j-config.jumpY+1,0);y<=j;++y){ //make sure there is no walls blocking the enemy from jumping
                            if(walls[x,y]) hjumpMaxX=-1;
                        }
                        for(x=i+2;x<hjumpMaxX;++x){
                            for(int y=Mathf.Max(0,j-config.jumpY+1);y<=j;++y){ //make sure there is no walls blocking the enemy from jumping
                                if(walls[x,y]){
                                    hjumpMaxX=-1;
                                    break;
                                }
                            }
                            if(hjumpMaxX!=-1 && grid_g[x,j]!=null){ //if there is a node on the right, connect the two nodes
                                grid_g[i,j].ConnectBothNode(grid_g[x,j]);
                                hjumpMaxX=-1;
                            }
                        }
                    }
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
        LayerMask layerMask=GameManager.inst.platformLayer;
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
    /// <summary>
    /// if needs jump to get from [from] node to a target node given that the two nodes are horizontal
    /// </summary>
    public bool NeedsJump(Node from, Node to){
        if(from.gridPos.y+1>=walls.GetLength(1)) return false;
        int x;
        if(to.gridPos.x>from.gridPos.x) x=from.gridPos.x+1;
        else x=from.gridPos.x-1;
        return !walls[x,from.gridPos.y+1];
    }
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

public class PathNavigator {
    EnemyBase ctrller;
    float chaseSpd;
    System.Func<bool> onGround;
    public Coroutine chaseCoro;

    public PathNavigator(EnemyBase ctrller, float chaseSpd, System.Func<bool> onGround){
        this.ctrller=ctrller;
        this.chaseSpd=chaseSpd;
        this.onGround=onGround;
    }
    List<PathFinder.Node> paths;
    Vector2 GetHorizontalJumpVelocity(Vector2 from, Vector2 to){
        //calculate velocity
        Vector2 v=new Vector2(ctrller.Dir==1?chaseSpd:-chaseSpd,0);
        float jumpTime=(to.x-from.x)/v.x;
        float gravity=ctrller.rgb.gravityScale*9.8f;
        v.y=gravity*jumpTime/2; //vy=gt/2
        v.y+=.1f; //even with exact value, the enemy still jumps lower than desired height
        return v;
    }
    Vector2 GetJumpVelocity_exact(Vector2 from, Vector2 to){
        RaycastHit2D hit = Physics2D.Raycast(to, Vector2.down, float.MaxValue, GameManager.inst.groundMixLayer);
        if(!hit){
            Debug.LogError("no platform to land on");
            return Vector2.zero;
        }
        //calculate the exact [to] position: just when the ctrller's corner touches the platform's corner
        Vector2 bcExtents=ctrller.bc.bounds.extents, bcOffset=ctrller.bc.offset;
        if(ctrller.Dir==1){ //jump right up
            to.x-=PathFinder.inst.GridSize.x;
        } else //jump left up
            to.x+=PathFinder.inst.GridSize.x;
        to.y=hit.point.y+bcExtents.y;
        //calculate velocity
        Vector2 v=Vector2.zero;
        float jumpTime=.5f;
        float h=to.y-from.y;
        float gravity=ctrller.rgb.gravityScale*9.8f;
        v.y=h/jumpTime+.5f*gravity*jumpTime; //v=h/t+.5*gt
        v.x=(to.x-from.x)/jumpTime+.1f;
        v.y+=1; //even with exact value, the enemy still jumps lower than desired height
        return v;
    }
    PathFinder.Node NearestNodeToTarget(Transform target){
        return PathFinder.inst.GetNeareastNode_g(target.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    PathFinder.Node NearestNodeToThis(){
        return PathFinder.inst.GetNeareastNode_g(ctrller.transform.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    void FindPath(Transform target){
        paths=PathFinder.inst.FindPath_g(ctrller.transform.position, target.position, -ctrller.bc.bounds.extents.y+ctrller.bc.offset.y+PathFinder.inst.GridSize.y/2);
    }
    bool CheckStucked(float from){
        return Time.time-from>10;
    }
    bool CheckStucked(Vector2 lastPos){
        if(lastPos==ctrller.rgb.position)
            Debug.Log("enemy is stucked");
        return lastPos==ctrller.rgb.position;
    }
    public IEnumerator Chase(Transform target){
        float epsilon=0.4f; //used to check whether two points are close enough
        WaitForSeconds detectInterval=new WaitForSeconds(.05f);
        FindPath(target);
        PathFinder.Node cur=paths[0],prev;
        //move from the first node to the last
        int i=1;
        //when i<paths.Count, the enemy moves to paths[i]. when i==paths.Count,
        //the enemy moves toward the player.
        for(;i<=paths.Count;){ 
            Vector2 v;
            Vector2 lastPos;
            if(i==paths.Count){ //reaches the nearest node to the player. now moves toward the player
                ctrller.Dir=PlayerShootingController.inst.transform.position.x>ctrller.transform.position.x?1:-1;
                v=new Vector2(ctrller.Dir==1?chaseSpd:-chaseSpd, ctrller.rgb.velocity.y);
                //moves directly toward the player.
                for(;Mathf.Abs(target.transform.position.x-ctrller.transform.position.x)>epsilon;){
                    v.y=ctrller.rgb.velocity.y;
                    ctrller.rgb.velocity=v;
                    lastPos=ctrller.rgb.position;
                    yield return detectInterval;
                    if(CheckStucked(lastPos))
                        break;
                }
                ctrller.rgb.velocity=Vector2.zero;
            } else{ //i<paths.Count. Move through even nodes in [paths]
                cur=paths[i];
                prev=paths[i-1];
                float moveStartTime=Time.time;
                ctrller.Dir=(cur.gridPos.x>prev.gridPos.x)?1:-1;
                if(cur.gridPos.y==prev.gridPos.y){
                    if(PathFinder.inst.NeedsJump(prev, cur)){ //horizontal jump
                        v=GetHorizontalJumpVelocity(ctrller.transform.position, cur.worldPos);
                        ctrller.rgb.velocity=v;
                        for(;Vector2.Distance(ctrller.transform.position, cur.worldPos)>epsilon;){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    } else{ //move horizontally
                        v=new Vector2(ctrller.Dir==1?chaseSpd:-chaseSpd, ctrller.rgb.velocity.y);
                        ctrller.rgb.velocity=v;
                        for(;Mathf.Abs(ctrller.transform.position.x-cur.worldPos.x)>epsilon;){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    }
                }
                else if(cur.gridPos.y>prev.gridPos.y){ //jump down
                    v=new Vector2(ctrller.Dir==1?chaseSpd:-chaseSpd, ctrller.rgb.velocity.y);
                    ctrller.rgb.velocity=v;
                    float edgeXPos;
                    if(ctrller.Dir==1){ //edge is on the right
                        float boundsLeft=ctrller.bc.bounds.min.x;
                        edgeXPos=Mathf.Max(prev.worldPos.x+PathFinder.inst.GridSize.x/2, cur.worldPos.x-ctrller.bc.bounds.size.x/2);
                        bool wasInAir=false;
                        for(;edgeXPos>=boundsLeft || (!wasInAir || !onGround());){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos<boundsLeft)
                                v.x=0;
                            boundsLeft=ctrller.bc.bounds.min.x;
                            if(!onGround()) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked in jump down. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    } else{
                        float boundsRight=ctrller.bc.bounds.max.x;
                        edgeXPos=Mathf.Min(prev.worldPos.x-PathFinder.inst.GridSize.x/2, cur.worldPos.x+ctrller.bc.bounds.size.x/2);
                        bool wasInAir=false;
                        for(;edgeXPos<=boundsRight || (!wasInAir || !onGround());){
                            ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                            if(edgeXPos>boundsRight)
                                v.x=0;
                            boundsRight=ctrller.bc.bounds.max.x;
                            if(!onGround()) wasInAir=true;
                            if(CheckStucked(moveStartTime)){
                                Debug.LogWarning("the enemy might be stucked in jump down. restart path finding");
                                break;
                            }
                            yield return detectInterval;
                        }
                    }
                } else{ //jump up
                    //v=GetJumpVelocity(ctrller.transform.position, cur.worldPos+
                    //    new Vector2(-ctrller.Dir*(PathFinder.inst.GridSize.x/2+ctrller.bounds.max.x),
                    //    ctrller.bounds.max.y-PathFinder.inst.GridSize.y/2));
                    v=GetJumpVelocity_exact(ctrller.transform.position, cur.worldPos);
                    ctrller.rgb.velocity=v;
                    for(;Vector2.Distance(ctrller.transform.position, cur.worldPos)>epsilon;){
                        if((ctrller.Dir==1&&ctrller.transform.position.x>cur.worldPos.x)||(ctrller.Dir==-1&&ctrller.transform.position.x<cur.worldPos.x)){
                            v.x=0;
                        }
                        if(v.x==0&&onGround()) break;
                        ctrller.rgb.velocity=new Vector2(v.x, ctrller.rgb.velocity.y);
                        if(CheckStucked(moveStartTime)){
                            Debug.LogWarning("the enemy might be stucked. restart path finding");
                            break;
                        }
                        yield return detectInterval;
                    }
                    while(!onGround())
                        yield return detectInterval;
                }
            }
            //detect if the target node changes
            PathFinder.Node targetNode=NearestNodeToTarget(target);
            PathFinder.Node fromNode=NearestNodeToThis();

            if(targetNode!=paths[^1] || fromNode !=paths[i-1]){
                paths=PathFinder.inst.FindPath_g(fromNode, targetNode);
                i=1;
            } else if(i<paths.Count) ++i; //this causes a loop because the enemy will chase the player until animator bool "b_attack" is set to true
            yield return 0;
        }
        ctrller.animator.SetTrigger("idle");
        ctrller.rgb.velocity=Vector2.zero;
    }
}