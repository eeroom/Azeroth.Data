// Code generated by a template
using System;
using Azeroth.Nalu;
namespace T4
{
    /// <summary>
    /// 待评估的人员的因素明细
    /// <summary>
    public class Tb_EvaluatedPeopleDetail
    {
        /// <summary>
        ///Id
        /// </summary>
        public Guid Id {set;get;}
        /// <summary>
        ///股权分配Id
        /// </summary>
        public Guid ShareHoldingId {set;get;}
        /// <summary>
        ///待评估人员Id
        /// </summary>
        public Guid EvaluatedPeopleId {set;get;}
        /// <summary>
        ///因素Id
        /// </summary>
        public Guid EvaluatedFactorId {set;get;}
        /// <summary>
        ///分数
        /// </summary>
        public Double Score {set;get;}
        /// <summary>
        ///创建时间
        /// </summary>
        public DateTime CreateTime {set;get;}
        /// <summary>
        ///创建人ID
        /// </summary>
        public Guid Creator {set;get;}
        /// <summary>
        ///是否删除
        /// </summary>
        public Boolean IsDel {set;get;}
    }
}
// It's the end