﻿namespace Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public Employee employee { get; set; }
    }
}