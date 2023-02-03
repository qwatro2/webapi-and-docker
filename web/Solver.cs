using Dapper;
using Npgsql;
using web.Models;

namespace web;

public class Solver
{
    public static Operation FromStringToOperation(char symbol)
    {
        return symbol switch
        {
            '+' => Operation.Plus,
            '-' => Operation.Minus,
            '*' => Operation.Multiplication,
            '/' => Operation.Division,
            _ => Operation.Undefined
        };
    }

    public static string Serialize(Dictionary<string, dynamic?> dictionary)
    {
        var res = "{ ";
        foreach (var kv in dictionary)
        {
            if (kv.Value is Dictionary<string, dynamic?>)
            {
                res += "{kv.Key}: { " + $"{Serialize(kv.Value)}" + " }; ";
            }
            else if (kv.Value is null)
            {
                res += $"{kv.Key}: null; ";
            }
            else
            {
                res += $"{kv.Key}: {kv.Value}; ";
            }
        }

        if (res.Length > 1)
        {
            res = res[..^2];
        }

        return res + " }";
    }

    public static async Task<int> CreateTable(NpgsqlConnection conn)
    {
        const string q = @"create table if not exists arguments
                (
                    id              bigserial
                        primary key,
                    first_argument  double precision,
                    second_argument double precision,
                    operation       varchar
                );

                alter table arguments
                    owner to postgres;



                create table if not exists public.requests
                (
                    id           bigserial
                        primary key,
                    request_type varchar not null,
                    request_name varchar not null,
                    arguments_id bigint
                        references public.arguments,
                    result       jsonb not null,
                    status_code  integer not null,
                    date         timestamp default now()
                );

                alter table public.requests
                    owner to postgres;

                ";
        return await conn.ExecuteAsync(q);
    }

}
    