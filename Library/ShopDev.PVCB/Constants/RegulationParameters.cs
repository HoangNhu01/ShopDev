using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CR.PVCB.Constants
{
    public class RegulationParameters
    {
        /// <summary>
        /// Quy tắc tạo tên tài khoản ảo
        /// </summary>
        #region nameRule
        public const string AccordingCustomerName = "01";
        public const string AccordingPaymentAcc = "02";
        public const string NameRuleManual = "99";
        #endregion

        /// <summary>
        /// Quy tắc tạo số tài khoản ảo
        /// </summary>
        #region numberRule
        public const string AccordingParameter = "01";
        public const string NumberRuleManual = "99";
        #endregion
    }
}
