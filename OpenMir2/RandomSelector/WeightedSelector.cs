using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMir2.RandomSelector
{
    /// <summary>
    /// 权重筛选器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightedSelector<T> : IEnumerable<T>
    {
        public readonly List<WeightedItem<T>> Items = new List<WeightedItem<T>>();
        public readonly SelectorOption Option;

        /// <summary>
        /// 累计权重集
        /// </summary>
        public int[] CumulativeWeights;

        /// <summary>
        /// 是否是已经添加过的权重值
        /// </summary>
        private bool _isAddedCumulativeWeights;

        public WeightedSelector(SelectorOption option = null)
        {
            Option = option ?? new SelectorOption();
        }

        public WeightedSelector(List<WeightedItem<T>> items, SelectorOption option = null) : this(option)
        {
            Add(items);
        }

        public WeightedSelector(IEnumerable<WeightedItem<T>> items, SelectorOption option = null) : this(option)
        {
            Add(items);
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="item"></param>
        public void Add(WeightedItem<T> item)
        {
            if (item.Weight <= 0)
            {
                if (Option.RemoveZeroWeightItems)
                {
                    return;
                }

                throw new InvalidOperationException("权重值不能为0");
            }

            _isAddedCumulativeWeights = true;
            Items.Add(item);
        }

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<WeightedItem<T>> items)
        {
            foreach (WeightedItem<T> item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="item"></param>
        /// <param name="weight"></param>
        public void Add(T item, int weight)
        {
            Add(new WeightedItem<T>(item, weight));
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="item"></param>
        public void Remove(WeightedItem<T> item)
        {
            _isAddedCumulativeWeights = true;
            Items.Remove(item);
        }

        /// <summary>
        /// 执行权重筛选，取一个元素
        /// </summary>
        public T Select()
        {
            CalculateCumulativeWeights();
            SingleSelector<T> selector = new SingleSelector<T>(this);
            return selector.Select();
        }

        /// <summary>
        /// 执行权重筛选，取多个元素
        /// </summary>
        public List<T> SelectMultiple(int count)
        {
            CalculateCumulativeWeights();
            MultipleSelector<T> selector = new MultipleSelector<T>(this);
            return selector.Select(count);
        }

        /// <summary>
        /// 计算累计权重
        /// </summary>
        private void CalculateCumulativeWeights()
        {
            if (!_isAddedCumulativeWeights) //如果没有被添加，则跳过
            {
                return;
            }

            _isAddedCumulativeWeights = false;
            CumulativeWeights = GetCumulativeWeights(Items);
        }

        /// <summary>
        /// 计算累计权重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int[] GetCumulativeWeights(List<WeightedItem<T>> items)
        {
            int totalWeight = 0;
            int index = 0;
            int[] results = new int[items.Count + 1];

            foreach (WeightedItem<T> item in items)
            {
                totalWeight += item.Weight;
                results[index] = totalWeight;
                index++;
            }

            return results;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator() as IEnumerator<T>;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}