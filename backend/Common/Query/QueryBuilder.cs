using System.Text;
using Dapper;

namespace quantity_move_api.Common.Query;

public class QueryBuilder
{
    private readonly StringBuilder _sql = new();
    private readonly DynamicParameters _parameters = new();
    private bool _hasWhere = false;

    public QueryBuilder Select(string columns)
    {
        _sql.Append($"SELECT {columns}");
        return this;
    }

    public QueryBuilder From(string table)
    {
        _sql.Append($" FROM {table}");
        return this;
    }

    public QueryBuilder Where(string condition, object? value = null)
    {
        if (!_hasWhere)
        {
            _sql.Append(" WHERE ");
            _hasWhere = true;
        }
        else
        {
            _sql.Append(" AND ");
        }

        _sql.Append(condition);
        
        if (value != null)
        {
            var paramName = ExtractParameterName(condition);
            if (!string.IsNullOrEmpty(paramName))
            {
                _parameters.Add(paramName, value);
            }
        }

        return this;
    }

    public QueryBuilder And(string condition, object? value = null)
    {
        return Where(condition, value);
    }

    public QueryBuilder Or(string condition, object? value = null)
    {
        if (!_hasWhere)
        {
            _sql.Append(" WHERE ");
            _hasWhere = true;
        }
        else
        {
            _sql.Append(" OR ");
        }

        _sql.Append(condition);
        
        if (value != null)
        {
            var paramName = ExtractParameterName(condition);
            if (!string.IsNullOrEmpty(paramName))
            {
                _parameters.Add(paramName, value);
            }
        }

        return this;
    }

    public QueryBuilder OrderBy(string columns)
    {
        _sql.Append($" ORDER BY {columns}");
        return this;
    }

    public QueryBuilder GroupBy(string columns)
    {
        _sql.Append($" GROUP BY {columns}");
        return this;
    }

    public QueryBuilder Having(string condition)
    {
        _sql.Append($" HAVING {condition}");
        return this;
    }

    public QueryBuilder Limit(int count)
    {
        _sql.Append($" TOP {count}");
        return this;
    }

    public QueryBuilder AddParameter(string name, object? value)
    {
        _parameters.Add(name, value);
        return this;
    }

    public QueryBuilder AddCondition(string condition)
    {
        if (!_hasWhere)
        {
            _sql.Append(" WHERE ");
            _hasWhere = true;
        }
        else
        {
            _sql.Append(" AND ");
        }
        _sql.Append(condition);
        return this;
    }

    public (string Sql, DynamicParameters Parameters) Build()
    {
        return (_sql.ToString(), _parameters);
    }

    public override string ToString()
    {
        return _sql.ToString();
    }

    private static string? ExtractParameterName(string condition)
    {
        // Extract parameter name from condition like "field = @ParamName"
        var match = System.Text.RegularExpressions.Regex.Match(condition, @"@(\w+)");
        return match.Success ? match.Groups[1].Value : null;
    }
}

