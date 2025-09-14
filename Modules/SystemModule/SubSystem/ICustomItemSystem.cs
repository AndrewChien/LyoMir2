namespace SystemModule.SubSystem
{
    public interface ICustomItemSystem
    {
        /// <summary>
        /// 增加自定义物品名称ItemNameList.txt
        /// </summary>
        /// <param name="nMakeIndex"></param>
        /// <param name="nItemIndex"></param>
        /// <param name="sItemName"></param>
        /// <returns></returns>
        bool AddCustomItemName(int nMakeIndex, int nItemIndex, string sItemName);
        /// <summary>
        /// 删除自定义物品名称ItemNameList.txt
        /// </summary>
        /// <param name="nMakeIndex"></param>
        /// <param name="nItemIndex"></param>
        void DelCustomItemName(int nMakeIndex, int nItemIndex);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMakeIndex"></param>
        /// <param name="nItemIndex"></param>
        /// <returns></returns>
        string GetCustomItemName(int nMakeIndex, int nItemIndex);
        /// <summary>
        /// 读取自定义物品名称ItemNameList.txt
        /// </summary>
        void LoadCustomItemName();
        /// <summary>
        /// 保存自定义物品名称ItemNameList.txt
        /// </summary>
        void SaveCustomItemName();
    }
}