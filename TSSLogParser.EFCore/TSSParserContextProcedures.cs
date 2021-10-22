﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using TSSLogParser.EFCore;

namespace TSSLogParser.EFCore
{
    public partial class TSSParserContext
    {
        private TSSParserContextProcedures _procedures;

        public TSSParserContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new TSSParserContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public TSSParserContextProcedures GetProcedures()
        {
            return Procedures;
        }
    }

    public partial class TSSParserContextProcedures
    {
        private readonly TSSParserContext _context;

        public TSSParserContextProcedures(TSSParserContext context)
        {
            _context = context;
        }

        public virtual async Task<int> FlatEventImportAsync(string AppServiceName, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "AppServiceName",
                    Size = 510,
                    Value = AppServiceName ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[FlatEventImport] @AppServiceName", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<int> RecommendationImportAsync(string ProviderName, string LogName, string TruncatedMessage, string MsftDocsSearch, string MsftDocsTopResult, string WebSearch, string WebTopResult, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "ProviderName",
                    Size = -1,
                    Value = ProviderName ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "LogName",
                    Size = -1,
                    Value = LogName ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "TruncatedMessage",
                    Size = -1,
                    Value = TruncatedMessage ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "MsftDocsSearch",
                    Size = -1,
                    Value = MsftDocsSearch ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "MsftDocsTopResult",
                    Size = 510,
                    Value = MsftDocsTopResult ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "WebSearch",
                    Size = -1,
                    Value = WebSearch ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                new SqlParameter
                {
                    ParameterName = "WebTopResult",
                    Size = 510,
                    Value = WebTopResult ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[RecommendationImport] @ProviderName, @LogName, @TruncatedMessage, @MsftDocsSearch, @MsftDocsTopResult, @WebSearch, @WebTopResult", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<List<RunSSISPkgXMLImportResult>> RunSSISPkgXMLImportAsync(string FilePath, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "FilePath",
                    Size = 510,
                    Value = FilePath ?? Convert.DBNull,
                    SqlDbType = SqlDbType.NVarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<RunSSISPkgXMLImportResult>("EXEC @returnValue = [dbo].[RunSSISPkgXMLImport] @FilePath", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}
