#define IKO132

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ExtensionMethods.MyExtensions;
using CM3D2.XtMasterSlave.Plugin;
using UnityEngine;
using System.Reflection;
using kt.ik;

namespace XtMasterSlave_IK_XDLL
{
#if IKO132
    public class IkpInst : IkInst
    {
        public const string INFO = "Fixed by @ghorsington on Twitter";

        public static bool boAnime = false; //?
        public static bool IKBend = false;  //

        public override bool IsNewPointIK(Maid m, string hand = "右手")
        {
            var ikP = m.body0.fullBodyIK.GetIKCtrl(hand).GetIKSettingData(AIKCtrl.IKAttachType.Point);
            return (ikP.attachType == AIKCtrl.IKAttachType.NewPoint);
        }

        public override object GetIkPoint(TBody body, string hand = "右手")
        {
#if DEBUG
            if (Input.GetKey(KeyCode.Space))
            {
                IKBend = true;
                Console.WriteLine("IKBend: ON");
            }
            else
            {
                IKBend = false;
            }
#endif
            var obj = body.fullBodyIK.GetIKCtrl(hand).GetIKSettingData(AIKCtrl.IKAttachType.Point);
            if (obj == null)
                obj = body.fullBodyIK.GetIKCtrl(hand).GetIKSettingData(AIKCtrl.IKAttachType.NewPoint);

            return obj;
        }

        public override object GetIkCtrl(Maid maid)
        {
            return maid.fullBodyIK;
        }

        public override object GetIkCtrlPoint(TBody body, string hand = "右手")
        {
#if DEBUG
            if (Input.GetKey(KeyCode.Space))
            {
                IKBend = true;
                Console.WriteLine("IKBend: ON");
            }
            else
            {
                IKBend = false;
            }
#endif
            var obj = body.fullBodyIK.GetIKCtrl(hand);
            return obj;
        }

        private AIKCtrl.IKAttachType GetDefType(XtMasterSlave.MsLinkConfig mscfg)
        {
            if (mscfg.doIK159NewPointToDef)
            {
                return AIKCtrl.IKAttachType.NewPoint;
            }
            else
            {
                return AIKCtrl.IKAttachType.Point;
            }
        }

        public override void IkClear(Maid tgt, XtMasterSlave.MsLinkConfig mscfg)
        {
            List<string> listHand = new List<string> { "右手", "左手" };
            IkClear(tgt, listHand, mscfg);
        }

        //public override void IkClear(Maid tgt, List<string> listHand, XtMasterSlave.MsLinkConfig mscfg, AIKCtrl.IKAttachType IkType = (AIKCtrl.IKAttachType)(-1))
        public override void IkClear(Maid tgt, List<string> listHand, XtMasterSlave.MsLinkConfig mscfg, int IkType = (-1))
        {
            List<AIKCtrl.IKAttachType> listTypes = new List<AIKCtrl.IKAttachType>
                                    { AIKCtrl.IKAttachType.NewPoint, AIKCtrl.IKAttachType.Rotate };

            listHand.ToList().ForEach(h =>
            {
                var ctrl = tgt.body0.fullBodyIK.GetIKCtrl(h);
                listTypes.ForEach(t =>
                {
                    var iks = ctrl.GetIKSettingData(t);

                    if (IkXT.IsIkCtrlO117)
                    {

                        IKAttachParam ikAttachParam = new IKAttachParam(null, tgt);
                        ikAttachParam.attachType = t;
                        ikAttachParam.execTiming = AIKCtrl.IKExecTiming.Normal;
                        ikAttachParam.offset = Vector3.zero;


                        ctrl.SetIKSetting(ikAttachParam);
                        //ctrl.SetIKSetting(t, AIKCtrl.IKExecTiming.Normal, null, string.Empty, null, Vector3.zero);
                        ctrl.Detach();
                        //ctrl.SetIKSetting(t, false, null, -1, string.Empty, null, null, Vector3.zero, false, 0f);
                        //iks.SetIKSetting(null, -1, string.Empty, null, null, Vector3.zero, false, 0f);
                        //ctrl.Detach(t, 0f);
                    }
                    else
                    {
                        //iks.TgtMaid = null;
                        //iks.Tgt_AttachSlot = -1;
                        //iks.Tgt_AttachName = string.Empty;
                        //iks.Target = null;
                        //iks.AxisTgt = null;
                        //iks.TgtOffset = Vector3.zero;
                        //iks.IsTgtAxis
                    }

                    if (iks.attachType != AIKCtrl.IKAttachType.Rotate)
                    {
                        if (IkType >= 0 && IkType != (int)AIKCtrl.IKAttachType.Rotate
                                && Enum.IsDefined(typeof(AIKCtrl.IKAttachType), IkType))
                        {
                            iks.attachType=((AIKCtrl.IKAttachType)IkType);
                        }
                        else
                        {
                            if (mscfg != null)
                                iks.attachType=(GetDefType(mscfg));/*fix v5.0
                            else
                                iks.ChangePointType(AIKCtrl.IKAttachType.NewPoint);*/
                        }
                    }
                });
            });
        }



public override void CopyHandIK(Maid master, Maid slave, XtMasterSlave.v3Offsets[] v3ofs, int num_)
        {
            List<string> listHand = new List<string> { "右手", "左手" };
            List<AIKCtrl.IKAttachType> listTypes = new List<AIKCtrl.IKAttachType>
                                    { AIKCtrl.IKAttachType.NewPoint, AIKCtrl.IKAttachType.Rotate };

            listHand.ToList().ForEach(h =>
            {
                var ikcm = master.body0.fullBodyIK.GetIKCtrl(h);
                var ikcs = slave.body0.fullBodyIK.GetIKCtrl(h);
                listTypes.ForEach(t =>
                {
                    var ikm = ikcm.GetIKSettingData(t);
                    var iks = ikcs.GetIKSettingData(t);

                    if (!(string.IsNullOrEmpty(ikm.curTargetData.tgtAttachName) && ikm.curTargetData.target == null))
                    {
                        //Console.WriteLine("{0} {1} -> {2} {3} {4}", h, t, ikm.attachType, ikm.Tgt_AttachName, ikm.Target);

                        if (iks.attachType != AIKCtrl.IKAttachType.Rotate)
                        {
                            if (ikm.attachType != AIKCtrl.IKAttachType.Rotate)
                            {
                                iks.attachType=(ikm.attachType);
                            }
                        }

                        float fixAngle(float angle)
                        {
                            while (Mathf.Abs(angle) > 360f)
                            {
                                angle = ((!(angle < 0f)) ? (angle - 360f) : (angle + 360f));
                            }
                            return angle;
                        }

                        if (IkXT.IsIkCtrlO117)
                        {
                            // befor
                            /*
                            ikcs.SetIKSetting(t
                                , AIKCtrl.IKExecTiming.Normal
                                , ikm.curTargetData.targetChara
                                , ikm.curTargetData.tgtAttachSlot
                                , ikm.curTargetData.tgtAttachName
                                , ikm.curTargetData.axisTarget
                                , ikm.curTargetData.target
                                , ikm.curTargetData.tgtOffset
                                , ikm.doAnimation);
                            */

                            // after
                            IKAttachParam ikAttachParam = new IKAttachParam( null, master);
                            ikAttachParam.attachType = t;
                            ikAttachParam.execTiming = AIKCtrl.IKExecTiming.Normal;
                            ikAttachParam.targetChara = ikm.curTargetData.targetChara;
                            ikAttachParam.slotName = ikm.curTargetData.tgtAttachName;
                            ikAttachParam.axisBone = ikm.curTargetData.axisTarget;
                            ikAttachParam.attachTarget = ikm.curTargetData.target;
                            ikAttachParam.offset = ikm.curTargetData.tgtOffset;
                            ikAttachParam.doAnimation = ikm.doAnimation;

                            ikcs.SetIKSetting(ikAttachParam);

                            //ikcs.SetIKSetting(t, AIKCtrl.IKExecTiming.Normal, ikm.curTargetData.targetChara, ikm.curTargetData.tgtAttachSlot, ikm.curTargetData.Tgt_AttachName, ikm.curTargetData.AxisTgt, ikm.curTargetData.Target, ikm.curTargetData.TgtOffset, ikm.DoAnimation);
                            //ikcs.SetIKSetting(t, false, ikm.TgtMaid, ikm.Tgt_AttachSlot, ikm.Tgt_AttachName, ikm.AxisTgt, ikm.Target, ikm.TgtOffset, ikm.DoAnimation, ikm.BlendTime);
                            //iks.SetIKSetting(ikm.TgtMaid, ikm.Tgt_AttachSlot, ikm.Tgt_AttachName, ikm.AxisTgt, ikm.Target, ikm.TgtOffset, ikm.DoAnimation, ikm.BlendTime);
                        }
                        else
                        {
                            //iks.TgtMaid = ikm.TgtMaid;
                            //iks.Tgt_AttachSlot = ikm.Tgt_AttachSlot;
                            //iks.Tgt_AttachName = ikm.Tgt_AttachName;
                            //iks.Target = ikm.Target;
                            //iks.AxisTgt = ikm.AxisTgt;
                        }

                        if (iks.isPointAttach)
                        {
                            iks.curTargetData.tgtOffset = ikm.curTargetData.tgtOffset;
                            if (h == "右手")
                                iks.curTargetData.tgtOffset += v3ofs[num_].v3HandROffset;
                            else
                                iks.curTargetData.tgtOffset += v3ofs[num_].v3HandLOffset;
                        }
                        else
                        {
                            Vector3 v3rot = Vector3.zero;
                            if (h == "右手")
                                v3rot = v3ofs[num_].v3HandROffsetRot;
                            else
                                v3rot = v3ofs[num_].v3HandLOffsetRot;

                            iks.curTargetData.tgtOffset.x = fixAngle(ikm.curTargetData.tgtOffset.x + v3rot.x);
                            iks.curTargetData.tgtOffset.y = fixAngle(ikm.curTargetData.tgtOffset.y + v3rot.y);
                            iks.curTargetData.tgtOffset.z = fixAngle(ikm.curTargetData.tgtOffset.z + v3rot.z);
                        }
                    }

                });
            });

            //needInit = true;
        }
        
        public override void SetHandIKRotate(string handName, Maid master, Maid slave, string boneTgtname, Vector3 v3HandLOffsetRot)
        {
            IKAttachParam ikAttachParam = new IKAttachParam(master, null);
            ikAttachParam.targetBoneName = boneTgtname;
            ikAttachParam.offset = v3HandLOffsetRot;
            ikAttachParam.attachType = AIKCtrl.IKAttachType.Rotate;
            ikAttachParam.doAnimation = boAnime;
            ikAttachParam.execTiming = AIKCtrl.IKExecTiming.Normal;

            slave.body0.fullBodyIK.IKAttach(handName, ikAttachParam);

            // befor
            /*
            slave.IKTargetToBone(
                  handName
                , master
                , boneTgtname
                , v3HandLOffsetRot
                , AIKCtrl.IKAttachType.Rotate
                , false
                , boAnime
                , AIKCtrl.IKExecTiming.Normal);
            */
            /**/
            //slave.IKTargetToBone(handName, master, boneTgtname, v3HandLOffsetRot, AIKCtrl.IKAttachType.Rotate, false, 0f, boAnime, false);
        }

        public override void SetHandIKTarget(XtMasterSlave.MsLinkConfig mscfg, string handName, Maid master, Maid slave, int slot_no, string attach_name, Transform target, Vector3 v3HandLOffset)
        {
            /*if (needInit)
            {
                needInit = false;
                if (mscfg.doIK159NewPointToDef)
                    IKInit(slave, mscfg);
#if DEBUG
                else
                    IKInit4OldPoint(slave);
#endif
            }*/
            IKAttachParam ikAttachParam = new IKAttachParam(slave, master);
            ikAttachParam.attachType = GetDefType(mscfg);
            ikAttachParam.execTiming = AIKCtrl.IKExecTiming.Normal;
            ikAttachParam.attachIKName = attach_name;
            ikAttachParam.attachTarget = target;
            ikAttachParam.offset = v3HandLOffset;
            ikAttachParam.doAnimation = boAnime;

            slave.fullBodyIK.GetIKCtrl(handName).SetIKSetting(ikAttachParam);
            //slave.fullBodyIK.GetIKCtrl(handName).SetIKSetting(GetDefType(mscfg), AIKCtrl.IKExecTiming.Normal, master, slot_no, attach_name, null, target, v3HandLOffset, boAnime);

            HandFootIKCtrl ikdata = slave.fullBodyIK.GetIKCtrl<HandFootIKCtrl>(handName);
            ikdata.correctType = HandFootIKCtrl.BorderCorrectType.Bone;
        }

        bool needInit = true;
        static string bodytgt = "dummyBodyTgtXt";
        //RootMotion.FinalIK.FullBodyBipedIK m_FullbodyIK;
#if DEBUG
        /*GameObject goBodyTgt = new GameObject("dummyBodyTgtXt");
        RootMotion.FinalIK.FullBodyBipedIK m_FullbodyIK;
        GameObject goTgtSR = new GameObject("xtTgtSR");
        GameObject goTgtSL = new GameObject("xtTgtSL");*/
#endif

        Dictionary<Maid, string> lastAnimeFNs = new Dictionary<Maid, string>();
        private void IKInit(Maid slave, XtMasterSlave.MsLinks ms, XtMasterSlave.MsLinkConfig mscfg)
        {
            var fik = slave.body0.fullBodyIK.GetNonPublicField<RootMotion.FinalIK.FullBodyBipedIK>("m_FullbodyIK");
            RootMotion.FinalIK.FullBodyBipedIK FullbodyIK = fik;
            var solver = FullbodyIK.solver;
            string[] tgtlist = new string[] { "IKTarget", "BendBone", "ChainRootBone" };

            bool animStop = true; //モーション停止中
            if (ms.doMasterSlave && !mscfg.doStackSlave_PosSyncMode)
            {
                animStop = false;
            }
            else
            {
                Animation anim = slave.body0.m_Bones.GetComponent<Animation>();
                animStop = !anim.isPlaying;
            }

            if (!lastAnimeFNs.ContainsKey(slave) || slave.body0.LastAnimeFN != lastAnimeFNs[slave])
            {
                lastAnimeFNs[slave] = slave.body0.LastAnimeFN;
                animStop = false;
            }

            solver.spineStiffness = 1f;      //背骨の硬さ
            solver.pullBodyVertical = 0.5f;  //ボディエフェクター位置補正
            solver.pullBodyHorizontal = 0f;
            solver.spineMapping.twistWeight = 0f;

            foreach (var e in solver.effectors)
            {
                if (animStop)
                {
                    e.positionWeight = 1f;
                    e.rotationWeight = 0f;
                }
                else
                {
                    e.PinToBone(1f, 0f);
                }

                var tgtname = e.target.gameObject.name;

                if (tgtlist.Contains(tgtname))
                {
                    // COM3D2標準ターゲット
                    e.target.transform.position = e.bone.position;
                    e.target.transform.rotation = e.bone.rotation;
                }
            }

            solver.rightShoulderEffector.positionWeight = 0.95f;
            solver.leftShoulderEffector.positionWeight = 0.95f;

            solver.bodyEffector.rotationWeight = 1f;

            solver.rightThighEffector.positionWeight = 0.95f;
            solver.leftThighEffector.positionWeight = 0.95f;

            if (mscfg != null && mscfg.doFinalIKShoulderMove)
            {
                solver.rightShoulderEffector.positionWeight = 0f;
                solver.leftShoulderEffector.positionWeight = 0f;
            }
            if (mscfg != null && mscfg.doFinalIKThighMove)
            { 
                solver.bodyEffector.rotationWeight = 0f;

                solver.rightThighEffector.positionWeight = 0f;
                solver.leftThighEffector.positionWeight = 0f;
            }

            foreach (var m in solver.limbMappings)
            {
                m.weight = 1f;
                m.maintainRotationWeight = 0f;
            }

            if (mscfg != null)
            {
                solver.rightLegMapping.weight = mscfg.fFinalIKLegWeight; //0.5f;
                solver.leftLegMapping.weight = mscfg.fFinalIKLegWeight; //0.5f;
            }
            solver.rightLegMapping.maintainRotationWeight = 1f;
            solver.leftLegMapping.maintainRotationWeight = 1f;
        }

#region for TEST
#if DEBUG
        private void IKInit4OldPoint(Maid slave)
        {
            //                    FullBodyIKMgr
            //var fik = slave.body0.fullBodyIK.GetNonPublicField<RootMotion.FinalIK.FullBodyBipedIK>("m_FullbodyIK");            
            //var fik = slave.body0.fullBodyIK.ik;
            RootMotion.FinalIK.FullBodyBipedIK FullbodyIK = slave.body0.fullBodyIK.ik;
            var solver = FullbodyIK.solver;
            string[] tgtlist = new string[] { "IKTarget", "BendBone", "ChainRootBone" };

            solver.spineStiffness = 1f;      //背骨の硬さ
            solver.pullBodyVertical = 0f;  //ボディエフェクター位置補正
            solver.pullBodyHorizontal = 0f;
            solver.spineMapping.twistWeight = 0f;

            foreach (var e in solver.effectors)
            {
                e.PinToBone(1f, 1f);

                var tgtname = e.target.gameObject.name;

                if (tgtlist.Contains(tgtname))
                {
                    // COM3D2標準ターゲット
                    e.target.transform.position = e.bone.position;
                    e.target.transform.rotation = e.bone.rotation;
                }
            }
            
            foreach(var m in solver.limbMappings)
            {
                m.weight = 0f;
                m.maintainRotationWeight = 1f;
            }
        }

        private void IKInit2(Maid slave)
        {
            var fik = slave.body0.fullBodyIK.GetNonPublicField<RootMotion.FinalIK.FullBodyBipedIK>("m_FullbodyIK");
            RootMotion.FinalIK.FullBodyBipedIK FullbodyIK = fik;
            var solver = FullbodyIK.solver;
            string[] tgtlist = new string[] { "IKTarget", "BendBone", "ChainRootBone" };
            string[] bendlist = new string[] { "BendBone", };
            /*
#if DEBUG
            solver.spineStiffness = 1f;      //背骨の硬さ
            //solver.rightLegMapping.weight = 0f;
            //solver.leftLegMapping.weight = 0f;
            if (!solver.bodyEffector.target || solver.bodyEffector.target.gameObject.name != bodytgt)
                solver.bodyEffector.target = new GameObject(bodytgt).transform;
            solver.bodyEffector.positionWeight = 1f;
            solver.bodyEffector.rotationWeight = 1f;
            solver.rightShoulderEffector.positionWeight = 0.95f;
            solver.leftShoulderEffector.positionWeight = 0.95f;
            solver.rightShoulderEffector.rotationWeight = 0.5f;
            solver.leftShoulderEffector.rotationWeight = 0.5f;

            solver.leftThighEffector.positionWeight = 1f;
            solver.leftThighEffector.rotationWeight = 0.5f;
            solver.rightThighEffector.positionWeight = 1f;
            solver.rightThighEffector.rotationWeight = 0.5f;
            solver.leftFootEffector.positionWeight = 1.0f;
            solver.rightFootEffector.positionWeight = 1.0f;

            if (!solver.rightThighEffector.target || solver.rightThighEffector.target.gameObject.name != bodytgt)
                solver.rightThighEffector.target = new GameObject(bodytgt).transform;
            if (!solver.leftThighEffector.target || solver.leftThighEffector.target.gameObject.name != bodytgt)
                solver.leftThighEffector.target = new GameObject(bodytgt).transform;

            if (!solver.rightFootEffector.target || solver.rightFootEffector.target.gameObject.name != bodytgt)
                solver.rightFootEffector.target = new GameObject(bodytgt).transform;
            if (!solver.leftFootEffector.target || solver.leftFootEffector.target.gameObject.name != bodytgt)
                solver.leftFootEffector.target = new GameObject(bodytgt).transform;

            solver.rightLegMapping.weight = 0.2f;
            solver.leftLegMapping.weight = 0.2f;
            //Sync(solver.leftArmChain.bendConstraint.bendGoal.transform, solver.leftArmMapping.bone2);
            //Sync(solver.rightArmChain.bendConstraint.bendGoal.transform, solver.rightArmMapping.bone2);
            foreach (var e in solver.effectors)
            {
                Sync(e);
                var tgtname = e.target.gameObject.name;

                if (tgtlist.Contains(tgtname) || tgtname == bodytgt)
                {
                    // COM3D2標準ターゲット
                    e.target.transform.position = e.bone.position;
                    e.target.transform.rotation = e.bone.rotation;
                }
                else if (bendlist.Contains(tgtname))
                {
                    // COM3D2標準ターゲット
                    e.target.transform.position = e.bone.position;
                    e.target.transform.rotation = e.bone.rotation;
                }
            }
            return;
#endif

            */
            solver.spineStiffness = 1f;      //背骨の硬さ
            solver.pullBodyVertical = 0.5f;  //ボディエフェクター位置補正
            solver.pullBodyHorizontal = 0f;

            foreach (var e in solver.effectors)
            {

                e.PinToBone(1f, 0f);
#if DEBUG
                e.PinToBone(1f, 1f);
#endif
                var tgtname = e.target.gameObject.name;

                if (tgtlist.Contains(tgtname))
                {
                    // COM3D2標準ターゲット
                    e.target.transform.position = e.bone.position;
                    e.target.transform.rotation = e.bone.rotation;
                }
            }
            solver.rightShoulderEffector.positionWeight = 0.95f;
            solver.leftShoulderEffector.positionWeight = 0.95f;
            solver.bodyEffector.rotationWeight = 1f;

            solver.rightLegMapping.maintainRotationWeight = 0f;
            solver.leftLegMapping.maintainRotationWeight = 0f;
            solver.rightLegMapping.weight = 0f;
            solver.leftLegMapping.weight = 0f;

            solver.rightArmMapping.maintainRotationWeight = 0f;
            solver.leftArmMapping.maintainRotationWeight = 0f;
            solver.rightArmMapping.weight = 1f;
            solver.leftArmMapping.weight = 1f;
        }

        private void IKInitTest(Maid slave, string handName)
        {
            var fik = slave.body0.fullBodyIK.GetNonPublicField<RootMotion.FinalIK.FullBodyBipedIK>("m_FullbodyIK");
            RootMotion.FinalIK.FullBodyBipedIK FullbodyIK = fik;

            var solver = FullbodyIK.solver;
            if (handName.Contains("右"))
            {
                //solver.rightLegMapping.weight = 0f;
                //solver.spineMapping.twistWeight = 0f;

                //m_FullbodyIK.references.root = slave.gameObject.transform;
                //solver.SetToReferences(FullbodyIK.references, null);
                solver.spineStiffness = 1f;
                solver.pullBodyVertical = 0f;
                solver.pullBodyHorizontal = 0f;

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    solver.spineStiffness = 0f;
                    solver.pullBodyVertical = 1f;
                }
                foreach (var e in solver.effectors)
                {
                    if (Input.GetKey(KeyCode.RightAlt))
                        e.PinToBone(0.25f, 1f);
                    else if (Input.GetKey(KeyCode.RightShift))
                        e.PinToBone(0.5f, 0f);
                    else if (Input.GetKey(KeyCode.RightControl))
                        e.PinToBone(0f, 1f);
                    else
                        e.PinToBone(1f, 1f);
                }
                solver.rightLegMapping.maintainRotationWeight = 1f;
                solver.leftLegMapping.maintainRotationWeight = 1f;

                if (Input.GetKey(KeyCode.Space))
                    solver.rightLegMapping.weight = 1f;
                else if (Input.GetKey(KeyCode.Keypad0))
                    solver.rightLegMapping.weight = 0f;
                else
                    solver.rightLegMapping.weight = 0.5f;

                /*Sync(solver.bodyEffector);
                Sync(solver.rightFootEffector);
                Sync(solver.rightThighEffector);
                Sync(solver.rightShoulderEffector);
                WeightZero(solver.rightFootEffector);
                WeightZero(solver.rightThighEffector);
                WeightZero(solver.rightShoulderEffector);*/
            }
            else
            {
                solver.leftLegMapping.maintainRotationWeight = 1f;
                if (Input.GetKey(KeyCode.Space))
                    solver.leftLegMapping.weight = 1f;
                else if (Input.GetKey(KeyCode.Keypad0))
                    solver.leftLegMapping.weight = 0f;
                else
                    solver.leftLegMapping.weight = 0.5f;

                /*Sync(solver.leftFootEffector);
                Sync(solver.leftThighEffector);
                Sync(solver.leftShoulderEffector);
                WeightZero(solver.leftFootEffector);
                WeightZero(solver.leftThighEffector);
                WeightZero(solver.leftShoulderEffector);*/
            }
        }
#endif
#endregion

        private static void Sync(Transform tr1, Transform tr2)
        {
            tr1.position = tr2.position;
            tr1.rotation = tr2.rotation;
        }
        private static void Sync(RootMotion.FinalIK.IKEffector eff)
        {
            eff.position = eff.bone.position;
            eff.rotation = eff.bone.rotation;
            /*eff.target.position = eff.bone.position;
            eff.target.rotation = eff.bone.rotation;
            eff.PinToBone(1f, 1f);*/
        }
        private static void WeightZero(RootMotion.FinalIK.IKEffector eff)
        {
            eff.positionWeight = 0f;
            eff.rotationWeight = 0f;
        }

        public override object GetIKCmo(TBody body, string hand = "右手")
        {
            return null;//body.fullBodyIK.GetIKCtrl(hand).IKCmo;

            /*
            if (hand == "右手")
                return body.fullBodyIK.GetIKCtrl("右手").IKCmo;
            else
                return body.fullBodyIK.GetIKCtrl("左手").IKCmo;
                */
        }

        public override bool IKUpdate(TBody body)
        {
            body.fullBodyIK.IKUpdate();
            return true;
        }

        public override bool GetIKCmoPosRot(TBody body, out Vector3 pos, out Quaternion rot, string hand = "右手")
        {
            var ctrl = body.fullBodyIK.GetIKCtrl(hand);
            bool proc = false;

            pos = Vector3.zero;
            rot = Quaternion.identity;

            var data = ctrl.GetIKSettingData(AIKCtrl.IKAttachType.Point);
            if (data.curTargetData.target != null)
            {
                pos = data.curTargetData.target.position;
                rot = data.curTargetData.target.rotation;
                proc = true;
            }
            else if (data.curTargetData.tgtAttachName != string.Empty)
            {
                if (data.curTargetData.targetChara != null && data.curTargetData.targetChara.body0 != null && data.curTargetData.tgtAttachSlot >= 0 && data.curTargetData.targetChara.body0.goSlot[data.curTargetData.tgtAttachSlot].morph != null)
                {
                    Vector3 vector;
                    data.curTargetData.targetChara.body0.goSlot[data.curTargetData.tgtAttachSlot].morph.GetAttachPoint(data.curTargetData.tgtAttachName, out pos, out rot, out vector, false);
                    proc = true;
                }
                else
                {
                    data.curTargetData.tgtAttachName = string.Empty;
                }
            }

            return proc;
        }

        public override bool IKCmoUpdate(TBody body, Transform trh, Vector3 offset, string hand = "右手")
        {
            var ctrl = body.fullBodyIK.GetIKCtrl(hand);
            /*ctrl.MyIKCtrl.GetType().GetProperty("IsUpdateEnd").SetValue(ctrl.MyIKCtrl, true, null);
            ctrl.MyIKCtrl.IsUpdateLate = false;
            ctrl.ApplyIKSetting();
            */

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            bool proc = GetIKCmoPosRot(body, out pos, out rot, hand);

            if (proc)
            {
                //ctrl.IKCmo.Porc(trh.parent.parent, trh.parent, trh, pos, rot * offset, ctrl);
                return true;
            }
            return false;
        }

        public override bool UpdateFinalIK(Maid maid, XtMasterSlave.MsLinks ms, XtMasterSlave.MsLinkConfig mscfg)
        {
            if (!maid || !maid.body0)
                return false;

            needInit = false;
            if (mscfg.doIK159NewPointToDef)
                IKInit(maid, ms, mscfg);
#if DEBUG
            else
                IKInit4OldPoint(maid);
#endif
            return true; // 実行できたか
        }
    }
#endif
}

public static class Extentions
{
    public static T GetNonPublicField<T>(this object obj, string name)
    {
        var ret = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        
        if (ret is T)
            return (T)ret;
        return default(T);
    }
}