namespace elchuycho.Darius
{
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Prediction;
    using System;
    using System.Linq;
    using EnsoulSharp.SDK.Utility;
    using Color = System.Drawing.Color;
    using SPrediction;

    public class Darius
    {
        public static Menu ComboM, HarassM, LaneM, JungleM, DrawM, KsM, MainMenu;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static int passiveCounter;


        public static AIHeroClient WAA
        {
            get { return ObjectManager.Player; }
        }

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E, 525);
            R = new Spell(SpellSlot.R, 460);
            E.SetSkillshot(0.20f, 100f, float.MaxValue, false, SkillshotType.Line);


            MainMenu = new Menu("elchuychoDarius", "elchuychoDarius", true);

            ComboM = new Menu("ComboSettings", "Combo Settings");
            ComboM.Add(new MenuBool("QC", "Use Q"));
            ComboM.Add(new MenuBool("WC", "Use W"));
            ComboM.Add(new MenuBool("EC", "Use E"));
            ComboM.Add(new MenuBool("RC", "Use R"));
            MainMenu.Add(ComboM);


            HarassM = new Menu("HarassSettings", "Harass Settings");
            HarassM.Add(new MenuBool("QH", "Use Q"));
            HarassM.Add(new MenuBool("WH", "Use W"));
            HarassM.Add(new MenuBool("EH", "Use E"));
            MainMenu.Add(HarassM);


            LaneM = new Menu("LaneSettings", "Lane Settings");
            LaneM.Add(new MenuBool("QL", "Use Q"));
            LaneM.Add(new MenuSlider("QL2", "Use Q When X Minions =", 3, 1, 5));
            LaneM.Add(new MenuBool("WL", "Use W"));
            LaneM.Add(new MenuBool("WL2", "Use W Under tower"));
            MainMenu.Add(LaneM);


            JungleM = new Menu("JungleSettings", "Jungle Settings");
            JungleM.Add(new MenuBool("QJ", "Use Q"));
            JungleM.Add(new MenuBool("WJ", "Use W"));
            JungleM.Add(new MenuBool("EJ", "Use E"));
            MainMenu.Add(JungleM);


            KsM = new Menu("KillStealSettings", "KillSteal Settings");
            KsM.Add(new MenuBool("QK", "Use Q"));
            KsM.Add(new MenuBool("RK", "Use R"));
            MainMenu.Add(KsM);


            DrawM = new Menu("DrawSettings", "Draw Settings");
            DrawM.Add(new MenuBool("QD", "Draw Q"));
            DrawM.Add(new MenuBool("WD", "Draw W"));
            DrawM.Add(new MenuBool("ED", "Draw E"));
            DrawM.Add(new MenuBool("RD", "Draw R"));
            MainMenu.Add(DrawM);


            Console.WriteLine("elchuycho Darius Loaded");
            Game.Print("ELCHUYCHO Darius Loaded!", System.Drawing.Color.Crimson);


            MainMenu.Attach();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }


        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;

            }
            KillSteal();

        }


        private static void Combo()
        {
            if (ComboM["QC"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range);
                if (target.IsValidTarget(Q.Range))
                {

                    MagnetQ(target);
                    Q.Cast();
                }

            }
            if (ComboM["WC"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target.IsValidTarget(W.Range))
                {
                    W.Cast(target);
                }

            }
            if (ComboM["EC"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range);
                if (target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }


            if (ComboM["RC"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.True);

                if (target.IsValidTarget(R.Range) && RDamage(target, passiveCounter) + PassiveDamage(target, 1) > target.Health)
                {
                    R.Cast(target);
                }

            }
        }



        private static void Harass()
        {
            if (HarassM["QH"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range);
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                }

            }
            if (HarassM["WH"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(target);
                }

            }
            if (HarassM["EH"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range);
                if (target.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.Cast(target);

                }
            }
        }


        private static void LaneClear()
        {
            if (LaneM["QL"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var minions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range) && x.IsMinion())
                            .Cast<AIBaseClient>().ToList();
                if (minions.Count >= LaneM["QL2"].GetValue<MenuSlider>())
                {
                    Q.CastOnUnit(minions.FirstOrDefault());
                }
            }
            if (LaneM["WL"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                foreach (var minions in GameObjects.EnemyMinions.Where(x =>
                   x.IsValidTarget(W.Range) && !x.IsInvulnerable && x.Health < W.GetDamage(x)))
                {
                    if (minions.Health <= W.GetDamage(minions))
                    {
                        W.CastOnUnit(minions);
                    }
                }
            }
            if (LaneM["WL2"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                foreach (var minions in GameObjects.EnemyTurrets.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.CastOnUnit(minions);
                }
            }
        }
        private static void KillSteal()
        {
            if (KsM["QK"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < WAA.GetSpellDamage(x, SpellSlot.Q)))
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }
                }
            }
            if (KsM["RK"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < WAA.GetSpellDamage(x, SpellSlot.R)))
                {
                    var target2 = TargetSelector.GetTarget(R.Range, DamageType.True);

                    if (target2.IsValidTarget(R.Range) && RDamage(target2, passiveCounter) + PassiveDamage(target2, 1) > target2.Health)
                    {
                        R.Cast(target2);
                    }
                }

            }
        }
        private static void JungleClear()
        {

            if (JungleM["QJ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(Q.Range)).FirstOrDefault(j => j.IsValidTarget(Q.Range));
                Q.CastOnUnit(jungleMonsters);
            }
            if (JungleM["WJ"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(W.Range)).FirstOrDefault(j => j.IsValidTarget(W.Range));
                W.CastOnUnit(jungleMonsters);
            }
            if (JungleM["EJ"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                var jungleMonsters = GameObjects.Jungle.Where(j => j.IsValidTarget(E.Range)).FirstOrDefault(j => j.IsValidTarget(E.Range));
                E.CastOnUnit(jungleMonsters);
            }
        }


        private static void OnDraw(EventArgs args)
        {
            if (DrawM["QD"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.FromArgb(0, 255, 255), 1);
            }
            if (DrawM["WD"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.FromArgb(0, 255, 255), 1);
            }
            if (DrawM["ED"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.FromArgb(0, 255, 255), 1);
            }
            if (DrawM["RD"].GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.FromArgb(0, 255, 255), 1);
            }
        }


        public static float RDamage(AIBaseClient unit, int stackcount)
        {
            var bonus = stackcount * (new[] { 20, 20, 40, 60 }[R.Level] + (0.15 * WAA.FlatPhysicalDamageMod));

            return
                (float)
                (bonus
                 + WAA.CalculateDamage(
                     unit,
                     DamageType.True,
                     new[] { 100, 100, 200, 300 }[R.Level] + (float)(0.75 * WAA.FlatPhysicalDamageMod)));
        }



        public static double PassiveDamage(AIBaseClient unit, int stackcount)
        {
            if (stackcount < 1)
            {
                stackcount = 1;
            }

            return WAA.CalculateDamage(
                unit,
                DamageType.Physical,
                (9 + WAA.Level) + (float)(0.3 * WAA.FlatPhysicalDamageMod)) * stackcount;
        }


        static void MagnetQ(AIBaseClient target)
        {

            if (target.Distance(WAA) <= Q.Range + 15)
            {
                var pos = Prediction.GetFastUnitPosition(target, Game.Ping / 2000f).Extend(WAA.Position, Q.Range - 100);
                WAA.IssueOrder(GameObjectOrder.MoveTo, pos.ToVector3(), false);
            }
        }




    }
}

