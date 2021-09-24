using FS.Data.Infrastructure;

namespace FS.Data.Internal
{
    public interface IContextConnection
    {
        /// <summary>
        ///     �����ַ���
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        ///     ���ݿ�����
        /// </summary>
        eumDbType DbType { get; set; }

        /// <summary>
        ///     ���ʱʱ��
        /// </summary>
        int CommandTimeout { get; set; }

        /// <summary>
        ///     ���ݿ�汾
        /// </summary>
        string DataVer { get; set; }
    }
}