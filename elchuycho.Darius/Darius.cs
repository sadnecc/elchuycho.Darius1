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


    public class Darius
    {
        public static Menu ComboM, HarassM, LaneM, JungleM, DrawM, KsM;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

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


            var MainMenu = new Menu("elchuycho:Darius", "elchuycho:Darius", true);

            ComboM = new Menu("Combo Settings", "Combo Settings");
            ComboM.Add(new MenuBool("QC", "Use Q"));
            ComboM.Add(new MenuBool("WC", "Use W"));
            ComboM.Add(new MenuBool("EC", "Use E"));
            ComboM.Add(new MenuBool("RC", "Use R"));
            MainMenu.Add(ComboM);


            HarassM = new Menu("Harass Settings", "Harass Settings");
            HarassM.Add(new MenuBool("QH", "Use Q"));
            HarassM.Add(new MenuBool("WH", "Use W"));
            HarassM.Add(new MenuBool("EH", "Use E"));
            MainMenu.Add(HarassM);


            LaneM = new Menu("Lane Settings", "Lane Settings");
            LaneM.Add(new MenuBool("QL", "Use Q"));
            LaneM.Add(new MenuSlider("QL2", "Use Q When X Minions =", 3, 1, 5));
            LaneM.Add(new MenuBool("WL", "Use W"));
            LaneM.Add(new MenuBool("WL2", "Use W Under tower"));
            MainMenu.Add(LaneM);


            JungleM = new Menu("Jungle Settings", "Jungle Settings");
            JungleM.Add(new MenuBool("QJ", "Use Q"));
            JungleM.Add(new MenuBool("WJ", "Use W"));
            JungleM.Add(new MenuBool("EJ", "Use E"));
            MainMenu.Add(JungleM);


            KsM = new Menu("KillSteal Settings", "KillSteal Settings");
            KsM.Add(new MenuBool("QK", "Use Q"));
            KsM.Add(new MenuBool("WK", "Use W"));
            KsM.Add(new MenuBool("RK", "Use R"));
            MainMenu.Add(KsM);


            DrawM = new Menu("Draw Settings", "Draw Settings");
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
                    KillSteal();
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    KillSteal();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    KillSteal();
                    break;
            }
        }


        private static void Combo()
        {
            if (ComboM["QC"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range);
                if (target.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.Cast(target);
                }

            }
            if (ComboM["WC"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target.IsValidTarget(W.Range) && W.IsReady())
                {
                    W.Cast(target);
                }

            }
            if (ComboM["EC"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                var target = TargetSelector.GetTarget(E.Range);
                if (target.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.Cast(target);
                }
            }
            if (ComboM["RC"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x =>
                   x.IsValidTarget(R.Range) && !x.IsInvulnerable && x.Health < R.GetDamage(x)))
                {
                    if (target.Health <= R.GetDamage(target))
                    {
                        R.CastOnUnit(target);
                    }
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
            if (KsM["WK"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && x.Health < WAA.GetSpellDamage(x, SpellSlot.W)))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        W.Cast(target);
                    }
                }
            }
            if (KsM["RK"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < WAA.GetSpellDamage(x, SpellSlot.R)))
                {
                    if (target.IsValidTarget(R.Range))
                    {
                        R.Cast(target);
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
    }
}

