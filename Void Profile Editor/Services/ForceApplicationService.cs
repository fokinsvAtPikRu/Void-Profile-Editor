using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Void_Profile_Editor.Abstraction;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Services
{
    public class ForceApplicationService : IForceApplicationService
    {
        public Result ApplyForces(Document document, Element element, List<NodeForceData> nodes)
        {
            if (document == null)
                return Result.Failure("Документ не инициализирован");

            if (element == null)
                return Result.Failure("Элемент не выбран");

            if (nodes == null || nodes.Count == 0)
                return Result.Failure("Нет данных для применения");

            try
            {
                using (Transaction tr = new Transaction(document, "Применение усилий"))
                {
                    tr.Start();

                    // Здесь логика записи усилий в параметры элемента
                    // Это пример - нужно адаптировать под ваши параметры

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        int nodeNumber = i + 1;

                        // Пример записи в параметры (нужно адаптировать под ваши имена параметров)
                        SetParameterValue(element, $"N_Узел{nodeNumber}", node.N);
                        SetParameterValue(element, $"Mx_Узел{nodeNumber}", node.Mx);
                        SetParameterValue(element, $"My_Узел{nodeNumber}", node.My);
                    }

                    tr.Commit();
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка при применении усилий: {ex.Message}");
            }
        }

        public Result<List<NodeForceData>> GetForces(Document document, Element element)
        {
            if (document == null)
                return Result.Failure<List<NodeForceData>>("Документ не инициализирован");

            if (element == null)
                return Result.Failure<List<NodeForceData>>("Элемент не выбран");

            try
            {
                var nodes = new List<NodeForceData>();

                // Определяем количество узлов по наличию параметров
                int maxNodeNumber = 0;
                foreach (Parameter param in element.Parameters)
                {
                    if (param.Definition.Name.StartsWith("N_Узел"))
                    {
                        var numberStr = param.Definition.Name.Replace("N_Узел", "");
                        if (int.TryParse(numberStr, out int nodeNumber))
                        {
                            maxNodeNumber = Math.Max(maxNodeNumber, nodeNumber);
                        }
                    }
                }

                // Читаем значения для каждого узла
                for (int i = 1; i <= maxNodeNumber; i++)
                {
                    var node = new NodeForceData($"Узел {i}", 0, 0, 0);

                    node.N = GetParameterValue(element, $"N_Узел{i}");
                    node.Mx = GetParameterValue(element, $"Mx_Узел{i}");
                    node.My = GetParameterValue(element, $"My_Узел{i}");

                    nodes.Add(node);
                }

                return Result.Success(nodes);
            }
            catch (Exception ex)
            {
                return Result.Failure<List<NodeForceData>>($"Ошибка при получении усилий: {ex.Message}");
            }
        }

        private void SetParameterValue(Element element, string paramName, double value)
        {
            Parameter param = element.LookupParameter(paramName);
            if (param != null && param.StorageType == StorageType.Double && !param.IsReadOnly)
            {
                param.Set(value);
            }
        }

        private double GetParameterValue(Element element, string paramName)
        {
            Parameter param = element.LookupParameter(paramName);
            if (param != null && param.StorageType == StorageType.Double && param.HasValue)
            {
                return param.AsDouble();
            }
            return 0;
        }
    }
}