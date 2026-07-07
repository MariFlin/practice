using System.Linq;
using System.Collections.Generic;

namespace task02
{
    public class StudentService
    {
        private readonly List<Student> _students;

        public StudentService(List<Student> students) => _students = students;

        // 1. Возвращает студентов указанного факультета
        public IEnumerable<Student> GetStudentsByFaculty(string faculty){   
            return _students
                .Where(s => s.Faculty == faculty)
                .ToList();
        }


        // 2. Возвращает студентов со средним баллом >= minAverageGrade
        public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade){
            return _students
                .Where(s => s.AverageGrade > minAverageGrade)
                .ToList();
        }

        // 3. Возвращает студентов, отсортированных по имени (A-Z)
        public IEnumerable<Student> GetStudentsOrderedByName(){
            return _students
                .OrderBy(s => s.Name)
                .ToList();
        }

        // 4. Группировка по факультету
        public ILookup<string, Student> GroupStudentsByFaculty(){
            return _students
                .ToLookup(s => s.Faculty);
        }

        // 5. Находит факультет с максимальным средним баллом
        public string GetFacultyWithHighestAverageGrade(){
            var result = _students
                .GroupBy(s => s.Faculty)
                .OrderByDescending(g => g.Average(s => s.AverageGrade))
                .FirstOrDefault();
            
            return result?.Key ?? "";
        }
    }
}