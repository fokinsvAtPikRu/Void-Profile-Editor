using Autodesk.Revit.DB;
using CSharpFunctionalExtensions;
using System.Collections.Generic;
using Void_Profile_Editor.Model;

namespace Void_Profile_Editor.Abstraction
{
    public interface IForceApplicationService
    {
        /// <summary>
        /// Применение усилий к узлам элемента
        /// </summary>
        /// <param name="document">Документ Revit</param>
        /// <param name="element">Элемент</param>
        /// <param name="nodes">Список узлов с усилиями</param>
        /// <returns>Результат операции</returns>
        Result ApplyForces(Document document, Element element, List<NodeForceData> nodes);

        /// <summary>
        /// Получение усилий из элемента
        /// </summary>
        /// <param name="document">Документ Revit</param>
        /// <param name="element">Элемент</param>
        /// <returns>Список узлов с усилиями или ошибка</returns>
        Result<List<NodeForceData>> GetForces(Document document, Element element);
    }
}