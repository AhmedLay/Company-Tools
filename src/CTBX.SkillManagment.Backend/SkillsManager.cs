using System.Collections.Immutable;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace CTBX.SkillManagment.Backend;

public class SkillsManager
{
    private readonly SkillManagerOptions _options;

    public SkillsManager(IOptions<SkillManagerOptions> options)
    {
        _options = options.Value;
    }

    public ImmutableList<Skill> GetSkills()
    {
        var sql = "SELECT Id, Name, Description, Type FROM PUBLIC.Skills";
        using var connection = new NpgsqlConnection(_options.ConnectionString);

        return connection.Query<Skill>(sql).ToImmutableList();
    }

    public void AddSkill(CreateSkill command)
    {
        var sql = "INSERT INTO PUBLIC.Skills(Name,Description,Type) VALUES (@Name,@Description,@Type)";
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Execute(sql, command);
    }

    public void RemoveSkill(RemoveSkill command)
    {
        var sql = "DELETE From PUBLIC.Skills WHERE id =@Id";
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Execute(sql, command);
    }

    public void EditSkill(EditSkill command)
    {
        var sql = "UPDATE PUBLIC.Skills  SET Name = @Name , Description = @Description , Type = @Type WHERE Id = @Id ";
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Execute(sql, command);
    }
}

