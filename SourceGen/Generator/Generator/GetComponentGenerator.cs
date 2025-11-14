using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

[Generator(LanguageNames.CSharp)]
public class GetComponentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 我们不依赖任何语法，直接无条件生成代码
        var source = SourceText.From(GenerateSource(), Encoding.UTF8);
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("ComponentExtensions_Get.g.cs", source);
        });
    }

    private static string GenerateSource()
    {
        return @"
using UnityEngine;

namespace UnityEngine
{
    public static partial class ComponentExtensions
    {
        /// <summary>
        /// 手写 SourceGen 版本的 GetComponent（不依赖任何特性糖衣）
        /// </summary>
        public static T Get<T>(this Component self) where T : Component
        {
            return self.GetComponent<T>();
        }

        public static T Get<T>(this GameObject self) where T : Component
        {
            return self.GetComponent<T>();
        }

        public static T GetInChildren<T>(this Component self, bool includeInactive = false) where T : Component
        {
            return self.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetInChildren<T>(this GameObject self, bool includeInactive = false) where T : Component
        {
            return self.GetComponentInChildren<T>(includeInactive);
        }

        public static T GetInParent<T>(this Component self, bool includeInactive = false) where T : Component
        {
            return self.GetComponentInParent<T>(includeInactive);
        }
    }
}
";
    }
}