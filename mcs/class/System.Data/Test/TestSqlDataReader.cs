//
// Test/SqlDataRead.cs
//
// Test to do read a simple forward read only record set.
// Using SqlCommand.ExecuteReader() to return a SqlDataReader
// which can be used to Read a row
// and Get a String or Int32.
//
// Author:
//	Daniel Morgan <danmorg@sc.rr.com>
//
// (C) 2002 Daniel Morgan
//

using System;
using System.Data;
using System.Data.SqlClient;

namespace TestSystemDataSqlClient {
	class TestSqlDataReader {

		static void Test(SqlConnection con, string sql, 
				CommandType cmdType, CommandBehavior behavior,
				string testDesc) 
		{ 
			SqlCommand cmd = null;
			SqlDataReader rdr = null;
			
			int c;
			int results = 0;

			Console.WriteLine("Test: " + testDesc);
			Console.WriteLine("[BEGIN SQL]");
			Console.WriteLine(sql);
			Console.WriteLine("[END SQL]");

			cmd = new SqlCommand(sql, con);
			cmd.CommandType = cmdType;
						
			Console.WriteLine("ExecuteReader...");
			rdr = cmd.ExecuteReader(behavior);

			do {
				results++;
				Console.WriteLine("Result Set " + results + "...");

				// get the DataTable that holds
				// the schema
				DataTable dt = rdr.GetSchemaTable();
                        			
				// number of columns in the table
				Console.WriteLine("   Total Columns: " +
					dt.Columns.Count);

				// display the schema
				foreach (DataRow schemaRow in dt.Rows) {
					foreach (DataColumn schemaCol in dt.Columns)
						Console.WriteLine(schemaCol.ColumnName + 
							" = " + 
							schemaRow[schemaCol]);
					Console.WriteLine();
				}

				int nRows = 0;
				string output, metadataValue, dataValue;
				// Read and display the rows
				Console.WriteLine("Gonna do a Read() now...");
				while(rdr.Read()) {
					Console.WriteLine("   Row " + nRows + ": ");
					
					for(c = 0; c < rdr.FieldCount; c++) {
						// column meta data 
						DataRow dr = dt.Rows[c];
						metadataValue = 
							"    Col " + 
							c + ": " + 
							dr["ColumnName"];
						
						// column data
						if(rdr.IsDBNull(c) == true)
							dataValue = " is NULL";
						else
							dataValue = 
								": " + 
								rdr.GetValue(c);
					
						// display column meta data and data
						output = metadataValue + dataValue;					
						Console.WriteLine(output);
					}
					nRows++;
				}
				Console.WriteLine("   Total Rows: " + 
						nRows);
			} while(rdr.NextResult());
			Console.WriteLine("Total Result sets: " + results);
			
			rdr.Close();
		}

		[STAThread]
		static void Main(string[] args) {
			String connectionString = null;
			connectionString = 
				"host=localhost;" +
				"dbname=test;" +
				"user=postgres";
						
			SqlConnection con;
			con = new SqlConnection(connectionString);
			con.Open();

			string sql;

			// Text - only has one query (single query behavior)
			sql = "select * from pg_tables";
			Test(con, sql, CommandType.Text, 
				CommandBehavior.SingleResult, "Text1");

			// Text - only has one query (default behavior)
			sql = "select * from pg_tables";
			Test(con, sql, CommandType.Text, 
				CommandBehavior.Default, "Text2");
			
			// Text - has three queries
			sql =
				"select * from pg_user;" + 
				"select * from pg_tables;" + 
				"select * from pg_database";
			Test(con, sql, CommandType.Text, 
				CommandBehavior.Default, "Text3Queries");
			
			// Table Direct
			sql = "pg_tables";
			Test(con, sql, CommandType.TableDirect, 
				CommandBehavior.Default, "TableDirect1");

			// Stored Procedure
			sql = "version";
			Test(con, sql, CommandType.StoredProcedure, 
				CommandBehavior.Default, "SP1");
			
			con.Close();
		}
	}
}
