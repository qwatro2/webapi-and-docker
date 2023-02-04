using System.Diagnostics;
using Dapper;
using Npgsql;

namespace web;

public static class Solver
{
    private static readonly string WorkingDirectory = Directory.GetCurrentDirectory() + @"\..";
    private const string Query =  @"create table if not exists arguments
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
                                        owner to postgres;";

    public static async Task CreateTable(NpgsqlConnection conn)
    {
        await conn.ExecuteAsync(Query);
    }
    
    public static void ExecuteCommand(string command)
    {
        using (var process = new Process
               {
                   StartInfo =
                   {
                       FileName = "docker-compose",
                       WorkingDirectory = WorkingDirectory,
                       Arguments = command
                   }
               })
        {
            process.Start();
            process.Close();
        }
        Thread.Sleep(2000);
    }
}
    