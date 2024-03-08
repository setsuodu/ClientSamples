# Samples

> 避开障碍物，找到最短路径抵达目标。

## 原理

*   Node 网格，网格越精细，寻路的效果越好，但计算量也越大
    *   G COST = 起始点到当前点的代价
    *   H COST = 目标点到当前点的代价（manhatten distance）
    *   F COST = G COST + H COST
*   Wall 障碍
*   OpenList 未计算代价的点 //开始只有StartPosition
*   CloseList 计算完代价的点 //开始为空

1.  不断遍历临近点（上、下、左、右）。
2.  如果该点是障碍，直接跳过。最终确定F COST最小的。
3.  每次迭代将open集合里F最小的点作为基点，对于基点周围的相邻点做如下处理：
    1.  如果这个点是障碍，直接无视。
    2.  如果这个点不在open表和close表中，则加入open表
    3.  如果这个点已经在open表中，并且当前基点所在路径代价更低，则更新它的G值和父亲
    4.  如果这个点在close表中，忽略。
        处理完之后将基点加入close集合。
4.  当终点出现在`OpenList`中，迭代结束。如果到达终点前`OpenList`表空了，说明没有路径可以到达终点。

## 阅读

*   [A星寻路算法入门](https://www.cnblogs.com/LiveForGame/p/10528393.html)

