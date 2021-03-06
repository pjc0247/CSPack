CSPack
====
Ruby compatible pack/unpack for .Net
<br>
http://ruby-doc.org/core-2.2.0/Array.html#method-i-pack

<br>
<br>
작업중

예시
----
```c#
var objs = new List<object> { 1234, "asdf" };
var packed = objs.Pack("Np");

var unpacked = packed.Unpack("Np");
```

디렉티브 추가하기
----
CSPack은 partial 클래스이며, CSPack 아래에 Packer 클래스를 구현하면 자동으로 인식됩니다.<br>
클래스는 `Packer_`이름으로 시작해야 하며, `_`뒤에는 디렉티브로 사용할 문자가 따라옵니다. 아래의 코드는 __k__에 대한 디렉티브를 생성합니다.
```c#
public static partial CSPack {
  
  public class Packer_k : PackerBase {
    public static object Unpack(Byte[] src, int offset, out int consumed)
    {
      // src : Unpack이 요청된 전체 바이트 배열입니다.
      // offset : src에 대한 현재 cursor입니다.
      // consumed : 이번 Unpack에서 몇 바이트를 사용했는지 기록해야 합니다.
      // return : 바이트 배열로부터 생성해낸 오브젝트를 반환해야 합니다.
    }
    public static Byte[] Pack(object obj)
    {
      // obj : Pack 해야 하는 대상 오브젝트입니다.
      // return : obj를 바이트 배열로 변환한 결과를 반환해야 합니다.
    }
  }
}
```
C# 문법상 당연한 일이지만, 상속을 통해 프록시 디렉티브를 만들 수 있습니다. 아래의 코드는 __k__ 디렉티브를 __u__ 디렉티브로 프록시합니다.
```c#
public class Packer_u : Packer_k { }
```
`i_`, `i!` 처럼 디렉티브 알파벳 뒤에 접미사가 붙는 경우 클래스 이름 맨 뒤에 `_`접미사를 붙이는 것으로 대응할 수 있습니다.<br>
이 경우 `public static List<String>`형의 `suffix` 필드를 만들어 허용되는 접미사를 명시해주어야 합니다.<br>
아래의 __k__ 디렉티브는 `_`, `!` 접미사를 허용합니다.
```c#
public class Packer_k_ : Packer_k {
  public static List<String> suffix = new List<String> { "_", "!" };
}
```
