using System.Data;
using System.Data.Common;

namespace KeMai.Data
{
    /// <summary>
    /// Wrapper IDbConnection class to allow for connection sharing, mocking, etc.
    /// </summary>
    public class DefaultConnection
        : IDbConnection, IHasDbConnection, IHasDbTransaction, ISetDbTransaction
    {
        public readonly DefaultConnectionFactory Factory;
        public IDbTransaction Transaction { get; set; }
        public IDbTransaction DbTransaction => Transaction;
        private IDbConnection dbConnection;

        public IDbDialectProvider DialectProvider { get; set; }
        public string LastCommandText { get; set; }
        public int? CommandTimeout { get; set; }

        public DefaultConnection(DefaultConnectionFactory factory)
        {
            this.Factory = factory;
            this.DialectProvider = factory.DialectProvider;
            this.connectionString = factory.ConnectionString;
        }

        public IDbConnection DbConnection => dbConnection ?? (dbConnection = ToDbConnection(ConnectionString,Factory.DialectProvider));

        public void Dispose()
        {
            Factory.OnDispose?.Invoke(this);
            if (!Factory.AutoDisposeConnection) return;

            DbConnection.Dispose();
            dbConnection = null;
        }

        /// <summary>
        /// ��ʼ���ݿ�����
        /// </summary>
        /// <returns> ��ʾ������Ķ���</returns>
        public IDbTransaction BeginTransaction()
        {
            if (Factory.AlwaysReturnTransaction != null)
                return Factory.AlwaysReturnTransaction;

            return DbConnection.BeginTransaction();
        }

        /// <summary>
        ///  ��ָ���� System.Data.IsolationLevel ֵ��ʼһ�����ݿ�����
        /// </summary>
        /// <param name="isolationLevel">System.Data.IsolationLevel ֵ֮һ��</param>
        /// <returns> ��ʾ������Ķ���</returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (Factory.AlwaysReturnTransaction != null)
                return Factory.AlwaysReturnTransaction;

            return DbConnection.BeginTransaction(isolationLevel);
        }

        public void Close()
        {
            DbConnection.Close();
        }
        /// <summary>
        /// Ϊ�򿪵� Connection ������ĵ�ǰ���ݿ⡣
        /// </summary>
        /// <param name="databaseName"> Ҫ���浱ǰ���ݿ����ʹ�õ����ݿ�����ơ�</param>
        public void ChangeDatabase(string databaseName)
        {
            DbConnection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// ����������һ���������������� Command ����
        /// </summary>
        /// <returns>һ���������������� Command ����</returns>
        public IDbCommand CreateCommand()
        {
            if (Factory.AlwaysReturnCommand != null)
                return Factory.AlwaysReturnCommand;

            var cmd = DbConnection.CreateCommand();

            return cmd;
        }

        /// <summary>
        ///  ��һ�����ݿ����ӣ����������ṩ�����ض��� Connection ����� ConnectionString ����ָ����
        /// </summary>
        public void Open()
        {
            if (DbConnection.State == ConnectionState.Broken)
                DbConnection.Close();

            if (DbConnection.State == ConnectionState.Closed)
            {
                DbConnection.Open();
                //so the internal connection is wrapped for example by miniprofiler
                if (Factory.ConnectionFilter != null)
                    dbConnection = Factory.ConnectionFilter(dbConnection);
            }
        }

        private string connectionString;
        /// <summary>
        ///  ��ȡ���������ڴ����ݿ���ַ�����
        ///  �����������õ��ַ�����
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString ?? Factory.ConnectionString; }
            set { connectionString = value; }
        }
        /// <summary>
        /// ��ȡ�ڳ��Խ�������ʱ��ֹ���Բ����ɴ���֮ǰ���ȴ���ʱ�䡣
        ///  �ȴ����Ӵ򿪵�ʱ�䣨����Ϊ��λ����Ĭ��ֵΪ 15 �롣
        /// </summary>
        public int ConnectionTimeout => DbConnection.ConnectionTimeout;
        /// <summary>
        /// ��ȡ��ǰ���ݿ�����Ӵ򿪺�Ҫʹ�õ����ݿ�����ơ�
        ///  ��ǰ���ݿ�����ƻ������Ӵ򿪺�Ҫʹ�õ����ݿ�����ơ�Ĭ��ֵΪ���ַ�����
        /// </summary>
        public string Database => DbConnection.Database;
        /// <summary>
        ///  ��ȡ���ӵĵ�ǰ״̬��
        ///   System.Data.ConnectionState ֵ֮һ��
        /// </summary>
        public ConnectionState State => DbConnection.State;
        /// <summary>
        /// 
        /// </summary>
        public bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConn"></param>

        public static explicit operator DbConnection(DefaultConnection dbConn)
        {
            return (DbConnection)dbConn.DbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnectionStringOrFilePath"></param>
        /// <param name="dialectProvider"></param>
        /// <returns></returns>
        public  IDbConnection ToDbConnection(string dbConnectionStringOrFilePath, IDbDialectProvider dialectProvider)
        {
            var dbConn = dialectProvider.CreateConnection(dbConnectionStringOrFilePath, options: null);
            return dbConn;
        }
    }

    internal interface ISetDbTransaction
    {
        IDbTransaction Transaction { get; set; }
    }
}