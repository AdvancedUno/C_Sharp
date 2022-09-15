using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ExampleChart
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //LiveCharts.Configure(config =>
            //    config
            //         registers SkiaSharp as the library backend
            //         REQUIRED unless you build your own
            //        .AddSkiaSharp()

            //         adds the default supported types
            //         OPTIONAL but highly recommend
            //        .AddDefaultMappers()

            //         select a theme, default is Light
            //         OPTIONAL
            //        .AddDarkTheme()
            //        .AddLightTheme()

            //         finally register your own mappers
            //         you can learn more about mappers at:
            //ToDo add website link...
            //        .HasMap<City>((city, point) =>
            //        {
            //            point.PrimaryValue = city.Population;
            //            point.SecondaryValue = point.Context.Index;
            //        })
            //     .HasMap<Foo>(....)
            //     .HasMap<Bar>(....)
            //    );
            }
    }
}
