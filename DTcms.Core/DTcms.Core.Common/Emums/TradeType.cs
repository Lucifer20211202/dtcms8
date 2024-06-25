using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.Common.Emums
{
    /// <summary>
    /// 交易类型
    /// </summary>
    public enum TradeType
    {
        /// <summary>
        /// 商品购买
        /// </summary>
        Goods = 0,
        /// <summary>
        /// 会员充值
        /// </summary>
        Recharge = 1,
        /// <summary>
        /// 会员订阅
        /// </summary>
        Subscription = 2
    }
}
