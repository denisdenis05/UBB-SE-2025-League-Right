using Duo.Data;
using Duo.Models.Quizzes;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Duo.Repositories;

public class QuizRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public QuizRepository(DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        try
        {
            var quizzes = new List<Quiz>();
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_GetAllQuizzes";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                quizzes.Add(new Quiz(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetInt32(reader.GetOrdinal("SectionId")),
                    reader.GetInt32(reader.GetOrdinal("OrderNumber"))
                ));
            }
            
            return quizzes;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while retrieving quizzes: {ex.Message}", ex);
        }
    }

    public async Task<Quiz> GetByIdAsync(int quizId)
    {
        if (quizId <= 0)
        {
            throw new ArgumentException("Quiz ID must be greater than 0.", nameof(quizId));
        }

        try
        {
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_GetQuizById";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@quizId", quizId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                return new Quiz(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetInt32(reader.GetOrdinal("SectionId")),
                    reader.GetInt32(reader.GetOrdinal("OrderNumber"))
                );
            }
            
            throw new KeyNotFoundException($"Quiz with ID {quizId} not found.");
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while retrieving quiz with ID {quizId}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Quiz>> GetBySectionIdAsync(int sectionId)
    {
        if (sectionId <= 0)
        {
            throw new ArgumentException("Section ID must be greater than 0.", nameof(sectionId));
        }

        try
        {
            var quizzes = new List<Quiz>();
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_GetQuizzesBySectionId";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@sectionId", sectionId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                quizzes.Add(new Quiz(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetInt32(reader.GetOrdinal("SectionId")),
                    reader.GetInt32(reader.GetOrdinal("OrderNumber"))
                ));
            }
            
            return quizzes;
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while retrieving quizzes for section {sectionId}: {ex.Message}", ex);
        }
    }

    public async Task<int> AddAsync(Quiz quiz)
    {
        if (quiz == null)
        {
            throw new ArgumentNullException(nameof(quiz));
        }

        if (quiz.SectionId <= 0)
        {
            throw new ArgumentException("Section ID must be greater than 0.", nameof(quiz));
        }

        if (quiz.OrderNumber < 0)
        {
            throw new ArgumentException("Order number cannot be negative.", nameof(quiz));
        }

        try
        {
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_AddQuiz";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@sectionId", quiz.SectionId);
            command.Parameters.AddWithValue("@orderNumber", quiz.OrderNumber);
            
            var newIdParam = new SqlParameter("@newId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);
            
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return (int)newIdParam.Value;
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new KeyNotFoundException($"Section with ID {quiz.SectionId} not found.", ex);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new InvalidOperationException($"Order number {quiz.OrderNumber} already exists in section {quiz.SectionId}.", ex);
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while adding quiz: {ex.Message}", ex);
        }
    }

    public async Task UpdateAsync(Quiz quiz)
    {
        if (quiz == null)
        {
            throw new ArgumentNullException(nameof(quiz));
        }

        if (quiz.Id <= 0)
        {
            throw new ArgumentException("Quiz ID must be greater than 0.", nameof(quiz));
        }

        if (quiz.SectionId <= 0)
        {
            throw new ArgumentException("Section ID must be greater than 0.", nameof(quiz));
        }

        if (quiz.OrderNumber < 0)
        {
            throw new ArgumentException("Order number cannot be negative.", nameof(quiz));
        }

        try
        {
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_UpdateQuiz";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@quizId", quiz.Id);
            command.Parameters.AddWithValue("@sectionId", quiz.SectionId);
            command.Parameters.AddWithValue("@orderNumber", quiz.OrderNumber);
            
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new KeyNotFoundException($"Quiz with ID {quiz.Id} not found.", ex);
        }
        catch (SqlException ex) when (ex.Number == 50002)
        {
            throw new KeyNotFoundException($"Section with ID {quiz.SectionId} not found.", ex);
        }
        catch (SqlException ex) when (ex.Number == 50003)
        {
            throw new InvalidOperationException($"Order number {quiz.OrderNumber} already exists in section {quiz.SectionId}.", ex);
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while updating quiz: {ex.Message}", ex);
        }
    }

    public async Task DeleteAsync(int quizId)
    {
        if (quizId <= 0)
        {
            throw new ArgumentException("Quiz ID must be greater than 0.", nameof(quizId));
        }

        try
        {
            using var connection = await _databaseConnection.CreateConnectionAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "sp_DeleteQuiz";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@quizId", quizId);
            
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new KeyNotFoundException($"Quiz with ID {quizId} not found.", ex);
        }
        catch (SqlException ex)
        {
            throw new Exception($"Database error while deleting quiz with ID {quizId}: {ex.Message}", ex);
        }
    }
} 