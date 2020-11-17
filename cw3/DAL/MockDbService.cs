using System.Collections.Generic;
using cw3.Models;

namespace cw3.Services
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students = new List<Student>
        {
            new Student{IdStudent=1, FirstName="Jan", LastName="Kowalski", IndexNumber="s1234"},
            new Student{IdStudent=2, FirstName="Anna", LastName="Malewski", IndexNumber="s2342"},
            new Student{IdStudent=3, FirstName="Krzysztof", LastName="Andrzejewicz", IndexNumber="s5432"}
        };

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
