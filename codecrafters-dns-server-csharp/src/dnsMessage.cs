using System;
using System.Linq;
using codecrafters_dns_server.src;

namespace codecrafters_dns_server.src;
public class DnsMessage
(byte[] udpPacket)
{
	private ushort Id
	{
		get; set;
	} = (ushort)((udpPacket[0] << 8) | udpPacket[1]);
	// ((udpPacket[2] >> 7) & 0x1) == 0;
	private bool IsQuery { get; set; } = (udpPacket[2] & 0x80) == 0;
	// udpPacket[2] >> 3
	private int OpCode { get; set; } = udpPacket[2] & 0x78 & 0xF;
	// udpPacket[2] >> 5) & 0x1
	private bool AuthoritativeAnswer { get; set; } = (udpPacket[2] & 0x4) == 1;
	// (udpPacket[2] >> 6) & 0x1
	private bool Truncation { get; set; } = (udpPacket[2] & 0x2) == 1;
	// (udpPacket[2] >> 7) & 0x1
	private bool RecursionDesired { get; set; } = (udpPacket[2] & 0x80) == 1;
	// (udpPacket[3] >> 7) & 0x1)
	private bool RecursionAvailable { get; set; } = (udpPacket[3] & 0x80) == 1;
	// (udpPacket[3] >> 4) & 0x0F
	private int ResponseCode { get; set; } = udpPacket[3] & 0xF;
	private int QuestionCount
	{
		get; set;
	} = (ushort)((udpPacket[4] << 8) | udpPacket[5]);
	private int AnswerRecordCount
	{
		get; set;
	} = (ushort)((udpPacket[6] << 8) | udpPacket[7]);
	private int AuthorityRecordCount
	{
		get; set;
	} = (ushort)((udpPacket[8] << 8) | udpPacket[9]);
	private int AdditionalRecordCount
	{
		get; set;
	} = (ushort)((udpPacket[10] << 8) | udpPacket[11]);
	public byte[] GetResponse()
	{
		// Convert the message to a byte array
		var response = new byte[12];
		response[0] = (byte)(Id >> 8);
		response[1] = (byte)(Id & 0xFF);
		IsQuery = false;
		response[2] =
			(byte)((IsQuery ? 0 : 1) << 7 | (OpCode & 0xF) << 3 |
				   (AuthoritativeAnswer ? 1 : 0) << 2 | (Truncation ? 1 : 0) << 1 |
				   (RecursionDesired ? 1 : 0));
		response[3] =
			(byte)((RecursionAvailable ? 1 : 0) << 7 | (ResponseCode & 0xF));
		response[4] = (byte)(QuestionCount >> 8);
		response[5] = (byte)(QuestionCount & 0xFF);
		response[6] = (byte)(AnswerRecordCount >> 8);
		response[7] = (byte)(AnswerRecordCount & 0xFF);
		response[8] = (byte)(AuthorityRecordCount >> 8);
		response[9] = (byte)(AuthorityRecordCount & 0xFF);
		response[10] = (byte)(AdditionalRecordCount >> 8);
		response[11] = (byte)(AdditionalRecordCount & 0xFF);
		return response;
	}
	public sealed override string ToString()
	{
		return $"DnsMessage:\n" + $"  Id: {Id}\n" + $"  IsQuery: {IsQuery}\n" +
			   $"  OpCode: {OpCode}\n" +
			   $"  AuthoritativeAnswer: {AuthoritativeAnswer}\n" +
			   $"  Truncation: {Truncation}\n" +
			   $"  RecursionDesired: {RecursionDesired}\n" +
			   $"  RecursionAvailable: {RecursionAvailable}\n" +
			   $"  ResponseCode: {ResponseCode}\n" +
			   $"  QuestionCount: {QuestionCount}\n" +
			   $"  AnswerRecordCount: {AnswerRecordCount}\n" +
			   $"  AuthorityRecordCount: {AuthorityRecordCount}\n" +
			   $"  AdditionalRecordCount: {AdditionalRecordCount}";
	}
}


/*class DnsMessage
{
	// Header
	public ushort Id { get; set; }
	public bool IsResponse { get; set; }
	public int OpCode { get; set; }
	public bool IsAuthoritative { get; set; }
	public bool IsTruncated { get; set; }
	public bool IsRecursionDesired { get; set; }
	public bool IsRecursionAvailable { get; set; }
	public int ResponseCode { get; set; }
	public int QuestionCount { get; set; }
	public int AnswerCount { get; set; }
	public int NameServerCount { get; set; }
	public int AdditionalCount { get; set; }
	public DnsMessage(byte[] data)
	{
		// Parse the data
		Id = (ushort)((data[0] << 8) | data[1]);
		IsResponse = (data[2] & 0x80) != 0;
		OpCode = (data[2] >> 3) & 0xF;
		IsAuthoritative = (data[2] & 0x4) != 0;
		IsTruncated = (data[2] & 0x2) != 0;
		IsRecursionDesired = (data[2] & 0x1) != 0;
		IsRecursionAvailable = (data[3] & 0x80) != 0;
		ResponseCode = data[3] & 0xF;
		QuestionCount = (data[4] << 8) | data[5];
		AnswerCount = (data[6] << 8) | data[7];
		NameServerCount = (data[8] << 8) | data[9];
		AdditionalCount = (data[10] << 8) | data[11];
	}
	public byte[] GetResponse()
	{
		// Convert the message to a byte array
		byte[] data = new byte[12];
		// Set the header values
		IsResponse = true;
		data[0] = (byte)(Id >> 8);
		data[1] = (byte)(Id & 0xFF);
		data[2] = (byte)((IsResponse ? 1 : 0) << 7 | (OpCode & 0xF) << 3 |
						 (IsAuthoritative ? 1 : 0) << 2 |
						 (IsTruncated ? 1 : 0) << 1 | (IsRecursionDesired ? 1 : 0));
		data[3] =
			(byte)((IsRecursionAvailable ? 1 : 0) << 7 | (ResponseCode & 0xF));
		data[4] = (byte)(QuestionCount >> 8);
		data[5] = (byte)(QuestionCount & 0xFF);
		data[6] = (byte)(AnswerCount >> 8);
		data[7] = (byte)(AnswerCount & 0xFF);
		data[8] = (byte)(NameServerCount >> 8);
		data[9] = (byte)(NameServerCount & 0xFF);
		data[10] = (byte)(AdditionalCount >> 8);
		data[11] = (byte)(AdditionalCount & 0xFF);
		// Return the byte array
		return data;
	}
}*/