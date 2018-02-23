﻿//
//  Author:
//    Natalia Portillo claunia@claunia.com
//
//  Copyright (c) 2017, © Claunia.com
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Data;
using System.Data.SQLite;

namespace apprepodbmgr.Core
{
    public class SQLite : DbCore
    {
        SQLiteConnection dbCon;

        #region implemented abstract members of DBCore
        public override bool OpenDb(string database, string server, string user, string password)
        {
            try
            {
                string dataSrc = $"URI=file:{database}";
                dbCon          = new SQLiteConnection(dataSrc);
                dbCon.Open();

                const string SQL = "SELECT * FROM apprepodbmgr";

                SQLiteCommand dbcmd        = dbCon.CreateCommand();
                dbcmd.CommandText          = SQL;
                SQLiteDataAdapter dAdapter = new SQLiteDataAdapter {SelectCommand = dbcmd};
                DataSet           dSet     = new DataSet();
                dAdapter.Fill(dSet);
                DataTable dTable = dSet.Tables[0];

                if(dTable.Rows.Count != 1) return false;

                if((long)dTable.Rows[0]["version"] != 1)
                {
                    dbCon = null;
                    return false;
                }

                DbOps = new DbOps(dbCon, this);

                return true;
            }
            catch(SQLiteException ex)
            {
                Console.WriteLine("Error opening DB.");
                Console.WriteLine(ex.Message);
                dbCon = null;
                return false;
            }
        }

        public override void CloseDb()
        {
            dbCon?.Close();

            DbOps = null;
        }

        public override bool CreateDb(string database, string server, string user, string password)
        {
            try
            {
                string dataSrc = $"URI=file:{database}";
                dbCon          = new SQLiteConnection(dataSrc);
                dbCon.Open();
                SQLiteCommand dbCmd = dbCon.CreateCommand();

                #if DEBUG
                Console.WriteLine("Creating apprepodbmgr table");
                #endif

                string sql        = "CREATE TABLE apprepodbmgr ( version INTEGER, name TEXT )";
                dbCmd.CommandText = sql;
                dbCmd.ExecuteNonQuery();

                sql =
                    "INSERT INTO apprepodbmgr ( version, name ) VALUES ( '1', 'Canary Islands Computer Museum' )";
                dbCmd.CommandText = sql;
                dbCmd.ExecuteNonQuery();

                #if DEBUG
                Console.WriteLine("Creating applications table");
                #endif
                dbCmd.CommandText = Schema.AppsTableSql;
                dbCmd.ExecuteNonQuery();

                #if DEBUG
                Console.WriteLine("Creating files table");
                #endif
                dbCmd.CommandText = Schema.FilesTableSql;
                dbCmd.ExecuteNonQuery();

                dbCmd.Dispose();
                dbCon = null;
                return true;
            }
            catch(SQLiteException ex)
            {
                Console.WriteLine("Error opening DB.");
                Console.WriteLine(ex.Message);
                dbCon = null;
                return false;
            }
        }

        public override IDbDataAdapter GetNewDataAdapter()
        {
            return new SQLiteDataAdapter();
        }

        public override long LastInsertRowId => dbCon.LastInsertRowId;
        #endregion
    }
}