using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORMLibrary
{
    public class ORMContext
    {
        private readonly string _connectionString;

        public ORMContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Create(User entity)
        {
            
            //Достаю из entity данные чтобы добавить их в sql запрос
            int ageEntity = entity.Age;
            string loginEntity = entity.Login;
            string usernameEntity = entity.Username;
            string passwordEntity = entity.Password;

            string sql = $"INSERT INTO users (age, login, username, password) VALUES ({ageEntity}, '{loginEntity}', '{usernameEntity}', '{passwordEntity}')";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                int number = command.ExecuteNonQuery();
                Console.WriteLine("Добавлено объектов: {0}", number);
            }

            //чтение только что добавленного объекта
            string sql2 = $"SELECT TOP 1 * FROM users WHERE age = {ageEntity} AND login = '{loginEntity}' AND username = '{usernameEntity}' AND password = '{passwordEntity}'";
            User userResult = new User();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql2, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    // выводим названия столбцов
                    Console.WriteLine("{0}\t{1}\t{2}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));

                    while (reader.Read()) // построчно считываем данные
                    {
                        int age = reader.GetInt32(0);
                        string login = reader.GetString(1);
                        string username = reader.GetString(2);
                        string password = reader.GetString(3);

                        Console.WriteLine("{0} \t{1} \t{2}", age, login, username, password);

                        //Сборка объекта в единый объект и его return
                        userResult.Age = age;
                        userResult.Login = login;
                        userResult.Username = username;
                        userResult.Password = password;

                    }
                }

                reader.Close();

                return userResult;
            }

        }

        public List<User> ReadByAge(int age)
        {
            List<User> listResult = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = $"SELECT * FROM users WHERE age = {age}";
                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //age уже знаем
                        string login = reader.GetString(1);
                        string username = reader.GetString(2);
                        string password = reader.GetString(3);

                        User user = new User();
                        user.Age = age;
                        user.Login = login;
                        user.Username = username;
                        user.Password = password;

                        listResult.Add(user);
                    }
                }
            }

            return listResult;
        }

        public List<User> ReadByAll()
        {
            List<User> listResult = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = $"SELECT * FROM users";
                SqlCommand command = new SqlCommand(sql, connection);
          

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int age = reader.GetInt32(0);
                        string login = reader.GetString(1);
                        string username = reader.GetString(2);
                        string password = reader.GetString(3);

                        User user = new User();
                        user.Age = age;
                        user.Login = login;
                        user.Username = username;
                        user.Password = password;

                        listResult.Add(user);

                    }
                }
            }
            return listResult;
        }

        public void UpdatePassword(User entity, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = $"UPDATE users SET password = {password}";
                SqlCommand command = new SqlCommand(sql, connection);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteByAge(int age)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = $"DELETE FROM users_ WHERE age = {age}";
                SqlCommand command = new SqlCommand(sql, connection);

                command.ExecuteNonQuery();
            }
        }
    }
}
