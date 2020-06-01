using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class SortState
    {
        public enum SortStates//Модель для сортировки списка User-Ов
        {
            LoginAsc,    // по логину по возрастанию
            LoginDesc,   // по логину по убыванию
            LevelAsc, // по доступу по возрастанию
            LevelDesc,    // по доступу по убыванию
        }
        public SortState.SortStates LoginSort { get; private set; } // значение для сортировки по логину
        public SortState.SortStates LevelSort { get; private set; }    // значение для сортировки по уровню доступа
        public SortState.SortStates Current { get; private set; }    // текущее значение сортировки

        public SortState(SortState.SortStates sortOrder)
        {
            LoginSort = sortOrder == SortState.SortStates.LoginAsc ? SortState.SortStates.LoginDesc : SortState.SortStates.LoginAsc;
            LevelSort = sortOrder == SortState.SortStates.LevelAsc ? SortState.SortStates.LevelDesc : SortState.SortStates.LevelAsc;
            //CompanySort = sortOrder == SortState.CompanyAsc ? SortState.CompanyDesc : SortState.CompanyAsc;
            Current = sortOrder;
        }
    }
}
