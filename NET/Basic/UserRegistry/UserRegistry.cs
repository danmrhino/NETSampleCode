using System;
using System.Data.SQLite;

namespace PersonRegistry
{
    class Program
    {
        class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string PhoneNumber { get; set; }
        }

        static void Main(string[] args)
        {
            // Connection string for SQLite database
            string connectionString = @"Data Source=SimpleDB.db;Version=3;";

            // Create table if not exists
            CreateDatabaseTable(connectionString);

            Console.WriteLine("SQLite CRUD Operations\n");

            Console.WriteLine("Menu:");
            Console.WriteLine("1. Create Person");
            Console.WriteLine("2. Update Person");
            Console.WriteLine("3. Exit");

            while (true)
            {
                Console.Write("Enter your choice: ");
                int choice;

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        try
                        {
                            var person = GetPersonFromUser();
                            CreatePerson(connectionString, person);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating person: {ex.Message}");
                        }
                        break;
                    case 2:
                        try
                        {
                            var person = GetPersonFromUser();
                            UpdatePerson(connectionString, person);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating person: {ex.Message}");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Exiting the program.");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }

        static void CreateDatabaseTable(string connectionString)
        {
            // SQL command to create a table if not exists
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS People (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Age INTEGER,
                    PhoneNumber TEXT
                );";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static Person GetPersonFromUser()
        {
            var person = new Person();

            Console.Write("Enter First Name: ");
            person.FirstName = Console.ReadLine();
            Console.Write("Enter Last Name: ");
            person.LastName = Console.ReadLine();
            Console.Write("Enter Age: ");
            if (!int.TryParse(Console.ReadLine(), out person.Age))
            {
                throw new ArgumentException("Invalid input. Age must be a number.");
            }
            Console.Write("Enter Phone Number: ");
            person.PhoneNumber = Console.ReadLine();

            return person;
        }

        static void CreatePerson(string connectionString, Person person)
        {
            // SQL command to insert a person
            string insertQuery = @"
                INSERT INTO People (FirstName, LastName, Age, PhoneNumber)
                VALUES (@FirstName, @LastName, @Age, @PhoneNumber);";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", person.FirstName);
                    command.Parameters.AddWithValue("@LastName", person.LastName);
                    command.Parameters.AddWithValue("@Age", person.Age);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Person created successfully.");
        }

        static void UpdatePerson(string connectionString, Person person)
        {
            Console.Write("Enter Id of the person to update: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid input. Id must be a number.");
                return;
            }

            // SQL command to update a person
            string updateQuery = @"
                UPDATE People
                SET FirstName = @FirstName, LastName = @LastName, Age = @Age, PhoneNumber = @PhoneNumber
                WHERE Id = @Id;";

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", person.FirstName);
                    command.Parameters.AddWithValue("@LastName", person.LastName);
                    command.Parameters.AddWithValue("@Age", person.Age);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No person found with the provided Id.");
                    }
                    else
                    {
                        Console.WriteLine("Person updated successfully.");
                    }
                }
            }
        }
    }
}
