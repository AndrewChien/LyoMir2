using SystemModule.Data;

namespace M2Server.Maps.AutoPath
{
    public class PathMap
    {
        protected PathcellSuccess[,] PathMapArray;
        /// <summary>
        /// 地图高(X最大值)
        /// </summary>
        protected int Height;
        /// <summary>
        /// 地图宽(Y最大值)
        /// </summary>        
        protected int Width;
        private readonly GetCostFunc _getCostFunc;
        protected TRect ClientRect;
        /// <summary>
        /// 寻找范围
        /// </summary>
        private readonly int _scopeValue;
        /// <summary>
        /// 开始寻路
        /// </summary>
        protected bool StartFind;

        public PathMap()
        {
            _scopeValue = 1000; // 寻路范围
            _getCostFunc = null;
        }

        // *************************************************************
        // 方向编号转为X方向符号
        // 7  0  1
        // 6  X  2
        // 5  4  3
        // *************************************************************
        private static short DirToDx(int direction)
        {
            short result;
            switch (direction)
            {
                case 0:
                case 4:
                    result = 0;
                    break;
                case 1:
                case 2:
                case 3:
                    result = 1;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        private static short DirToDy(int direction)
        {
            short result;
            switch (direction)
            {
                case 2:
                case 6:
                    result = 0;
                    break;
                case 3:
                case 4:
                case 5:
                    result = 1;
                    break;
                default:
                    result = -1;
                    break;
            }
            return result;
        }

        // *************************************************************
        // 从TPathMap中找出 TPath
        // *************************************************************
        protected PointInfo[] FindPathOnMap(short x, short y, bool run)
        {
            int nCount = 0;
            short nX = LoaclX(x);
            short nY = LoaclY(y);
            if ((nX < 0) || (nY < 0) || (nX >= ClientRect.Right - ClientRect.Left) || (nY >= ClientRect.Bottom - ClientRect.Top))
            {
                return null;
            }
            if ((PathMapArray.Length <= 0) || (PathMapArray[nY, nX].Distance < 0))
            {
                return null;
            }
            PointInfo[] result = new PointInfo[PathMapArray[nY, nX].Distance + 1];
            while (PathMapArray[nY, nX].Distance > 0)
            {
                if (!StartFind)
                {
                    break;
                }
                result[PathMapArray[nY, nX].Distance] = new PointInfo(nX, nY);
                int direction = PathMapArray[nY, nX].Direction;
                nX = (short)(nX - DirToDx(direction));
                nY = (short)(nY - DirToDy(direction));
                nCount++;
            }
            result[0] = new PointInfo(nX, nY);
            if (run)
            {
                result = WalkToRun(result);
            }
            return result;
        }

        // 把WALK合并成RUN
        private static byte WalkToRunGetNextDirection(int sx, int sy, int dx, int dy)
        {
            int flagx;
            int flagy;
            const int drUp = 0;
            const int drUpright = 1;
            const int drRight = 2;
            const int drDownright = 3;
            const int drDown = 4;
            const int drDownleft = 5;
            const int drLeft = 6;
            const int drUpleft = 7;
            byte result = drDown;
            if (sx < dx)
            {
                flagx = 1;
            }
            else if (sx == dx)
            {
                flagx = 0;
            }
            else
            {
                flagx = -1;
            }
            if (Math.Abs(sy - dy) > 2)
            {
                if ((sx >= dx - 1) && (sx <= dx + 1))
                {
                    flagx = 0;
                }
            }
            if (sy < dy)
            {
                flagy = 1;
            }
            else if (sy == dy)
            {
                flagy = 0;
            }
            else
            {
                flagy = -1;
            }
            if (Math.Abs(sx - dx) > 2)
            {
                if ((sy > dy - 1) && (sy <= dy + 1))
                {
                    flagy = 0;
                }
            }
            if ((flagx == 0) && (flagy == -1))
            {
                result = drUp;
            }
            if ((flagx == 1) && (flagy == -1))
            {
                result = drUpright;
            }
            if ((flagx == 1) && (flagy == 0))
            {
                result = drRight;
            }
            if ((flagx == 1) && (flagy == 1))
            {
                result = drDownright;
            }
            if ((flagx == 0) && (flagy == 1))
            {
                result = drDown;
            }
            if ((flagx == -1) && (flagy == 1))
            {
                result = drDownleft;
            }
            if ((flagx == -1) && (flagy == 0))
            {
                result = drLeft;
            }
            if ((flagx == -1) && (flagy == -1))
            {
                result = drUpleft;
            }
            return result;
        }

        private PointInfo[] WalkToRun(PointInfo[] path)
        {
            int I;
            PointInfo[] result = null;
            if ((path != null) && (path.Length > 1))
            {
                PointInfo[] walkPath = path;
                int nStep = 0;
                I = 0;
                while (true)
                {
                    if (!StartFind)
                    {
                        break;
                    }
                    if (I >= walkPath.Length)
                    {
                        break;
                    }
                    if (nStep >= 2)
                    {
                        int nDir1 = WalkToRunGetNextDirection(walkPath[I - 2].nX, walkPath[I - 2].nX, walkPath[I - 1].nX, walkPath[I - 1].nX);
                        int nDir2 = WalkToRunGetNextDirection(walkPath[I - 1].nX, walkPath[I - 1].nX, walkPath[I].nX, walkPath[I].nX);
                        if (nDir1 == nDir2)
                        {
                            walkPath[I - 1].nX = -1;
                            walkPath[I - 1].nX = -1;
                            nStep = 0;
                        }
                        else
                        {
                            // 需要转向不能合并
                            I -= 1;
                            nStep = 0;
                            continue;
                        }
                    }
                    nStep++;
                    I++;
                }
                int n01 = 0;
                for (I = 0; I < walkPath.Length; I++)
                {
                    if ((walkPath[I].nX != -1) && (walkPath[I].nX != -1))
                    {
                        n01++;
                        result = new PointInfo[n01];
                        result[n01 - 1] = walkPath[I];
                    }
                }
                return result;
            }
            if ((path != null) && (path.Length > 0))
            {
                result = new PointInfo[path.Length - 1];
                for (I = 0; I < path.Length; I++)
                {
                    result[I - 1] = path[I];
                }
            }
            else
            {
                result = null;
            }
            return result;
        }

        // 把WALK合并成RUN
        // *************************************************************
        // 寻路算法
        // X1,Y1为路径运算起点，X2，Y2为路径运算终点
        // *************************************************************
        protected int MapX(int x)
        {
            return x + ClientRect.Left;
        }

        protected int MapY(int y)
        {
            return y + ClientRect.Top;
        }

        private short LoaclX(short x)
        {
            return (short)(x - ClientRect.Left);
        }

        private short LoaclY(short y)
        {
            return (short)(y - ClientRect.Top);
        }

        private void GetClientRect(int x, int y)
        {
            ClientRect = new TRect(0, 0, Width, Height);
            // Bounds定义一个矩形
            if (Width > _scopeValue)
            {
                ClientRect.Left = Math.Max(0, x - _scopeValue / 2);
                ClientRect.Right = ClientRect.Left + Math.Min(Width, x + _scopeValue / 2);
            }
            if (Height > _scopeValue)
            {
                ClientRect.Top = Math.Max(0, y - _scopeValue / 2);
                ClientRect.Bottom = ClientRect.Top + Math.Min(Height, y + _scopeValue / 2);
            }
        }

        public void GetClientRect(int x1, int y1, int x2, int y2)
        {
            int x;
            int y;
            if (x1 > x2)
            {
                x = x2 + (x1 - x2) / 2;
            }
            else if (x1 < x2)
            {
                x = x1 + (x2 - x1) / 2;
            }
            else
            {
                x = x1;
            }
            if (y1 > y2)
            {
                y = y2 + (y1 - y2) / 2;
            }
            else if (y1 < y2)
            {
                y = y1 + (y2 - y1) / 2;
            }
            else
            {
                y = y1;
            }
            ClientRect = new TRect(0, 0, Width, Height);
            if (Width > _scopeValue)
            {
                ClientRect.Left = Math.Max(0, x - _scopeValue / 2);
                ClientRect.Right = ClientRect.Left + Math.Min(Width, x + _scopeValue / 2);
            }
            if (Height > _scopeValue)
            {
                ClientRect.Top = Math.Max(0, y - _scopeValue / 2);
                ClientRect.Bottom = ClientRect.Top + Math.Min(Height, y + _scopeValue / 2);
            }
        }

        /// <summary>
        /// 初始化PathMapArray
        /// </summary>
        /// <param name="result"></param>
        private void FillPathMapPreparePathMap(ref PathcellSuccess[,] result)
        {
            int nWidth = ClientRect.Right - ClientRect.Left;
            int nHeight = ClientRect.Bottom - ClientRect.Top;
            result = new PathcellSuccess[nHeight, nWidth];
            for (int y = 0; y < nHeight; y++)
            {
                for (int x = 0; x < nWidth; x++)
                {
                    result[y, x] = new PathcellSuccess();
                    result[y, x].Distance = -1;
                }
            }
        }

        // 计算相邻8个节点的权cost，并合法点加入NewWave(),并更新最小cost
        // 合法点是指非障碍物且Result[X，Y]中未访问的点
        private void FillPathMapTestNeighbours(Wave oldWave, Wave newWave, ref PathcellSuccess[,] result)
        {
            for (int i = 0; i < 8; i++)
            {
                short x = (short)(oldWave.Item.X + DirToDx(i));
                short y = (short)(oldWave.Item.Y + DirToDy(i));
                int c = GetCost(x, y, i);
                if ((c >= 0) && (result[y, x].Distance < 0))
                {
                    newWave.Add(x, y, c, i);
                }
            }
        }

        private static void FillPathMapExchangeWaves(Wave oldWave, Wave newWave)
        {
            Wave w = oldWave;
            newWave = w;
            newWave.Clear();
        }

        // *************************************************************
        // 寻路算法
        // X1,Y1为路径运算起点，X2，Y2为路径运算终点
        // *************************************************************
        protected PathcellSuccess[,] FillPathMap(short x1, short y1, short x2, short y2)
        {
            PathcellSuccess[,] result = null;
            Wave oldWave;
            Wave newWave;
            bool finished;
            WaveCell I;
            GetClientRect(x1, y1);
            short nX1 = LoaclX(x1);
            short nY1 = LoaclY(y1);
            short nX2 = LoaclX(x2);
            short nY2 = LoaclY(y2);
            if (x2 < 0)
            {
                nX2 = x2;
            }
            if (y2 < 0)
            {
                nY2 = y2;
            }
            if ((x2 >= 0) && (y2 >= 0))
            {
                if ((Math.Abs(nX1 - nX2) > (ClientRect.Right - ClientRect.Left)) || (Math.Abs(nY1 - nY2) > (ClientRect.Bottom - ClientRect.Top)))
                {
                    result = new PathcellSuccess[0, 0];
                    return result;
                }
            }
            FillPathMapPreparePathMap(ref result);
            // 初始化PathMapArray ,Distance:=-1
            oldWave = new Wave();
            newWave = new Wave();
            try
            {
                result[nY1, nX1].Distance = 0;// 起点Distance:=0
                oldWave.Add(nX1, nY1, 0, 0);// 将起点加入OldWave
                FillPathMapTestNeighbours(oldWave, newWave, ref result);
                finished = (nX1 == nX2) && (nY1 == nY2);// 检验是否到达终点
                while (!finished)
                {
                    FillPathMapExchangeWaves(oldWave, newWave);
                    if (!StartFind)
                    {
                        break;
                    }
                    if (!oldWave.Start())
                    {
                        break;
                    }
                    do
                    {
                        if (!StartFind)
                        {
                            break;
                        }
                        I = oldWave.Item;
                        I.Cost = I.Cost - oldWave.MinCost;
                        // 如果大于MinCost
                        if (I.Cost > 0)
                        {
                            // 加入NewWave
                            // 更新Cost= cost-MinCost
                            newWave.Add(I.X, I.Y, I.Cost, I.Direction);
                        }
                        else
                        {
                            // 处理最小COST的点
                            if (result[I.Y, I.X].Distance >= 0)
                            {
                                continue;
                            }
                            result[I.Y, I.X].Distance = result[I.Y - DirToDy(I.Direction), I.X - DirToDx(I.Direction)].Distance + 1;
                            // 此点 Distance:=上一个点Distance+1
                            result[I.Y, I.X].Direction = I.Direction;
                            finished = (I.X == nX2) && (I.Y == nY2);
                            // 检验是否到达终点
                            if (finished)
                            {
                                break;
                            }
                            FillPathMapTestNeighbours(oldWave, newWave, ref result);
                        }
                    } while (!!oldWave.Next());
                }
            }
            finally
            {

            }
            return result;
        }

        protected virtual int GetCost(int x, int y, int direction)
        {
            int result;
            direction = direction & 7;
            if ((x < 0) || (x >= Width) || (y < 0) || (y >= Height))
            {
                result = -1;
            }
            else
            {
                result = _getCostFunc(x, y, direction, 0);
            }
            return result;
        }
    }
}