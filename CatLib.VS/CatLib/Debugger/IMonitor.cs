﻿/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using CatLib.API.Debugger;

namespace CatLib.Debugger
{
    /// <summary>
    /// 监控器
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// 设定监控处理器
        /// </summary>
        /// <param name="moitorName">监控名</param>
        /// <param name="handler">处理器</param>
        void SetMonitorHandler(string moitorName, IMonitorHandler handler);

        /// <summary>
        /// 监控一个内容
        /// </summary>
        /// <param name="monitorName">监控名</param>
        /// <param name="value">监控值</param>
        void Monitor(string monitorName, object value);
    }
}
