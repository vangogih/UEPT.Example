using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataSakura.Runtime.Bootstrap;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace DataSakura.Editor
{
    public sealed class AALinkXmlGenerationPreprocess : IPreprocessBuildWithReport
    {
        private static readonly Type[] _runtimeAssemblyTypes = Assembly.GetAssembly(typeof(BootstrapScope)).GetTypes();

        public int callbackOrder { get; }

        [MenuItem("DataSakura/Build/Generate AA link.xml", priority = 300)]
        public static void TestPreprocess()
        {
            var instance = new AALinkXmlGenerationPreprocess();
            instance.OnPreprocessBuild(null);
        }

        public void OnPreprocessBuild(BuildReport _)
        {
            var linkXmlGenerator = new UnityEditor.Build.Pipeline.Utilities.LinkXmlGenerator();
            

            linkXmlGenerator.AddTypes(typeof(VContainer.Internal.ContainerLocal<>),
                typeof(UniversalRenderPipelineAsset));
            linkXmlGenerator.AddTypes(GetTypesFromLifetimeScope());
            linkXmlGenerator.Save("Assets/_Project/link.xml");
        }

        private static List<Type> GetTypesInheritedFrom<T>()
        {
            var typesToPreserve = new List<Type>();

            try {
                Type genericType = typeof(T);

                if (genericType.IsInterface) {
                    foreach (Type type in _runtimeAssemblyTypes) {
                        if (type.IsClass && !type.IsAbstract && genericType.IsAssignableFrom(type))
                            typesToPreserve.Add(type);
                    }
                }

                if (genericType.IsClass) {
                    foreach (Type type in _runtimeAssemblyTypes) {
                        if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(genericType))
                            typesToPreserve.Add(type);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError($"FATAL ERROR: AA link.xml generation failed with message: {e.Message}");
                throw;
            }
            return typesToPreserve;
        }

        private static List<Type> GetTypesFromNamespace(string partOfNameSpace, Type[] typesFromAssembly)
        {
            var typesToPreserve = new List<Type>();

            try {
                foreach (Type type in typesFromAssembly)
                    if (type.Namespace != null && type.Namespace.Contains(partOfNameSpace))
                        typesToPreserve.Add(type);
            }
            catch (Exception e) {
                Debug.LogError($"FATAL ERROR: AA link.xml generation failed with message: {e.Message}");
                throw;
            }
            return typesToPreserve;
        }

        private static List<Type> GetTypesFromLifetimeScope()
        {
            var scopes = new List<LifetimeScope>();

            var go = new GameObject("toDelete");
            var scriptables = new List<ScriptableObject>();
            List<Type> typesToPreserve = new List<Type>();

            try {
                foreach (Type type in _runtimeAssemblyTypes.Where(myType =>
                             myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(LifetimeScope))))
                    scopes.Add((LifetimeScope)go.AddComponent(type));

                var builder = new ContainerBuilder();

                foreach (LifetimeScope scope in scopes) {
                    foreach (MethodInfo method in scope.GetType().GetTypeInfo().DeclaredMethods) {
                        if (method.Name == "Configure") {
                            foreach (FieldInfo field in scope.GetType().GetTypeInfo().DeclaredFields) {
                                if (field.GetValue(scope) == null && field.GetCustomAttribute<SerializeField>() != null) {
                                    if (field.FieldType.IsSubclassOf(typeof(MonoBehaviour))) {
                                        field.SetValue(scope, go.AddComponent(field.FieldType));
                                        continue;
                                    }

                                    if (field.FieldType.IsSubclassOf(typeof(ScriptableObject))) {
                                        var fValue = ScriptableObject.CreateInstance(field.FieldType);
                                        scriptables.Add(fValue);
                                        field.SetValue(scope, fValue);
                                        continue;
                                    }
                                }
                            }
                            method.Invoke(scope, new object[] { builder });
                        }
                    }
                }

                var registrationBuilders = (List<RegistrationBuilder>)builder.GetType()
                                                                             .GetTypeInfo()
                                                                             .DeclaredFields.First(f => f.Name == "registrationBuilders")
                                                                             .GetValue(builder);

                foreach (RegistrationBuilder readyBuilder in registrationBuilders) {
                    Type toAdd;

                    if (readyBuilder is ComponentRegistrationBuilder componentBuilder) {
                        toAdd = (Type)componentBuilder.GetType()
                                                      .GetTypeInfo()
                                                      .BaseType.GetTypeInfo()
                                                      .DeclaredFields.First(f => f.Name == "ImplementationType")
                                                      .GetValue(componentBuilder);
                    }
                    else {
                        toAdd = (Type)readyBuilder.GetType()
                                                  .GetTypeInfo()
                                                  .DeclaredFields.First(f => f.Name == "ImplementationType")
                                                  .GetValue(readyBuilder);
                    }

                    typesToPreserve.Add(toAdd);
                }
            }
            catch (Exception e) {
                Debug.LogError($"FATAL ERROR: AA link.xml generation failed with message: {e}");
                throw;
            }
            finally {
                Object.DestroyImmediate(go);

                foreach (ScriptableObject o in scriptables)
                    Object.DestroyImmediate(o);
            }

            return typesToPreserve;
        }
    }
}