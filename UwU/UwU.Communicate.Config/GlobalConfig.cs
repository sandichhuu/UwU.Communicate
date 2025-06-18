namespace UwU.Communicate.Config
{
    public static class GlobalConfig
    {
        public const bool TRACE_COMMUNICATE_DEBUG = true;

        // 4MB (Giảm đi thì sẽ tăng tốc độ, tuy nhiên có thể sẽ thiếu, có thể giảm về tới 256kB sẽ rất nhanh)
        public const int BUFFER_SIZE = 1024 * 4;

        // Nén dữ liệu sẽ làm kích thước gói tin gửi / nhận nhỏ hơn -> tốc độ gửi cao hơn nhưng CPU sẽ hoạt động nhiều hơn.
        /// <summary>
        /// Cài đặt nén dữ liệu
        /// [0] Không nén
        /// [1] Thuật toán Snappier
        /// [2] Thuật toán LZMA 7z (chưa hỗ trợ thời điểm này)
        /// Giá trị này không cần đồng bộ giữa server và client, khác nhau vẫn xử lý được
        /// </summary>
        public const int USE_COMPRESSSION = 1;
    }
}