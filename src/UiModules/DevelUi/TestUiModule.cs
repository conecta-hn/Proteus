/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Reporting;
using System;
using System.Windows.Controls;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.Proteus.Annotations.InteractionType;

[assembly: Name("TestUi Module")]

namespace TheXDS.Proteus
{
    public class TestUiModule : UiModule
    {
        public TestUiModule()
        {
            App.UiInvoke(SetupDashboard);
        }

        private void SetupDashboard()
        {
            ModuleDashboard = Misc.Internal.BuildWarning(
                "Este módulo está destinado únicamente a pruebas y al equipo de" +
                " desarrollo de Proteus, por lo que no debe ser distribuido en un" +
                " entorno de producción. César Morgan se absuelve de toda" +
                " responsabilidad por los daños que el uso indebido que este" +
                " módulo pueda causar.");
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void TestUi(object sender, EventArgs e)
        {
            ProteusLib.MessageTarget?.Show("Test");
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void OpenTestPage(object sender, EventArgs e)
        {
            Host.OpenPage<TestPage>();
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void OpenUiTestPage(object sender, EventArgs e)
        {
            Host.OpenPage<UITestPlayground>();
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void OpenTestMessage(object sender, EventArgs e)
        {
            ProteusLib.MessageTarget?.Show("Este es un mensaje de prueba");
            ProteusLib.MessageTarget?.Info("Este es un mensaje de prueba");
            ProteusLib.MessageTarget?.Warning("Este es un mensaje de prueba");
            ProteusLib.MessageTarget?.Stop("Este es un mensaje de prueba");
            ProteusLib.MessageTarget?.Error("Este es un mensaje de prueba");
            ProteusLib.MessageTarget?.Critical("Este es un mensaje de prueba");
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void TestQuestion(object sender, EventArgs e)
        {
            
            var r = MessageSplash.Ask("¿Funciona bien Proteus?","Esta es una ventana de prueba. ¿Ha funcionado bien?");
            ProteusLib.MessageTarget?.Info(r ? "Proteus funcionó bien c:" : "Nooooo!! Proteus no funcionó :c");
        }

        [InteractionItem, Essential, InteractionType(Operation)]
        public void TestInputSplash(object sender, EventArgs e)
        {
            static void Test<T>()
            {
                if (InputSplash.GetNew<T>("Introduzca un valor de prueba", out var v))
                {
                    ProteusLib.MessageTarget?.Show("Prueba de InputSplash", v?.ToString());
                }
            }

            Test<int>();
            Test<string>();
            Test<decimal>();
            Test<bool>();
            Test<DateTime>();
            Test<Range<DateTime>>();
        }

        [InteractionItem, Essential, InteractionType(Reports)]
        public void ReportTest(object sender, EventArgs e)
        {
            var model = typeof(User);
            var fs = new[]
            {
                new ContainsFilter(){ Value = "ev", Property = model.GetProperty("Id",typeof(string)) }
            };

            var q = QueryBuilder.BuildQuery(model, fs);

            var fd = ReportBuilder.MakeReport("Test");
            ReportBuilder.MakeTable(fd, q, new[] { model.GetProperty("Id",typeof(string)), model.GetProperty("Name"), model.GetProperty("DefaultGranted") });
            fd.Print("Test");
        }
    }
}