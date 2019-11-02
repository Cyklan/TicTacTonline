namespace Models
{
    public enum ResponseCode
    {
        Ok,
        UnplannedError,
        PlannedError,
        GameTurnProcessed,
        GameTie,
        GameOver,
        JoinedRoom,
        LeftRoom
    }

    public class Response
    {
        public ResponseHeader Header { get; set; }
        public Document Body { get; set; }
    }
}
