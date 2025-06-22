using System.CommandLine;
using System.CommandLine.Parsing;
using System.Runtime.CompilerServices;

namespace Nemonuri.RoslynQuoters;

/// <summary>
/// <see cref="Command"/>에 대한 이론입니다.
/// </summary>
public static class CommandTheory
{
    /// <summary>
    /// 대상 커맨드 인스턴스에 인자를 추가합니다.
    /// </summary>
    /// <typeparam name="TCommand"><see cref="Command"/> 또는 그것을 상속받은 타입</typeparam>
    /// <param name="command">대상 커맨드 인스턴스</param>
    /// <param name="argument">추가할 인자</param>
    /// <returns>대상 커맨드 인스턴스를 반환</returns>
    public static TCommand With<TCommand>(this TCommand command, Argument argument)
        where TCommand : Command
    {
        command.Add(argument);
        return command;
    }

    /// <summary>
    /// 대상 커맨드 인스턴스에 옵션을 추가합니다.
    /// </summary>
    /// <typeparam name="TCommand"><see cref="Command"/> 또는 그것을 상속받은 타입</typeparam>
    /// <param name="command">대상 커맨드 인스턴스</param>
    /// <param name="option">추가할 옵션</param>
    /// <returns>대상 커맨드 인스턴스를 반환</returns>
    public static TCommand With<TCommand>(this TCommand command, Option option)
        where TCommand : Command
    {
        command.Add(option);
        return command;
    }

    /// <summary>
    /// 대상 커맨드 인스턴스에 자식 커맨드를 추가합니다.
    /// </summary>
    /// <typeparam name="TCommand"><see cref="Command"/> 또는 그것을 상속받은 타입</typeparam>
    /// <param name="command">대상 커맨드 인스턴스</param>
    /// <param name="childCommand">추가할 자식 커맨드</param>
    /// <returns>대상 커맨드 인스턴스를 반환</returns>
    public static TCommand With<TCommand>(this TCommand command, Command childCommand)
        where TCommand : Command
    {
        command.Add(childCommand);
        return command;
    }

    /// <summary>
    /// 대상 커맨드 인스턴스에 검증 로직을 추가합니다.
    /// </summary>
    /// <typeparam name="TCommand"><see cref="Command"/> 또는 그것을 상속받은 타입</typeparam>
    /// <param name="command">대상 커맨드 인스턴스</param>
    /// <param name="validator">추가할 검증 로직</param>
    /// <returns>대상 커맨드 인스턴스를 반환</returns>
    public static TCommand WithValidator<TCommand>(this TCommand command, Action<CommandResult> validator)
        where TCommand : Command
    {
        command.Validators.Add(validator);
        return command;
    }

    /// <summary>
    /// 커맨드 구문 분석 결과로부터 특정 타입의 인스턴스를 만드는 로직을, 대상 커맨드 인스턴스에 추가합니다.
    /// </summary>
    /// <typeparam name="TCommand"><see cref="Command"/> 또는 그것을 상속받은 타입</typeparam>
    /// <typeparam name="TItem">인스턴스를 만들 특정 타입</typeparam>
    /// <param name="command">대상 커맨드 인스턴스</param>
    /// <param name="factory">커맨드 구분 분석 결과로부터 특정 타입의 인스턴스를 만드는 대리자</param>
    /// <param name="itemBox">로직 실행 후 만들어진 인스턴스를 담을 상자</param>
    /// <returns>대상 커맨드 인스턴스를 반환</returns>
    public static TCommand WithFactoryAction<TCommand, TItem>
    (
        this TCommand command,
        ParseResultToItemFactory<TItem> factory,
        StrongBox<TItem?> itemBox
    )
        where TCommand : Command
    {
        command.SetAction(ActionCore);
        return command;

        int ActionCore(ParseResult parseResult)
        {
            try
            {
                var exitCode = factory(parseResult, out TItem? item);
                itemBox.Value = item;
                return exitCode;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return 1;
            }
        }
    }
}

/// <summary>
/// 커맨드 구분 분석 결과로부터 특정 타입의 인스턴스를 만드는 대리자입니다.
/// </summary>
/// <typeparam name="TItem">인스턴스를 만들 특정 타입</typeparam>
/// <param name="parseResult">커맨드 구분 분석 결과</param>
/// <param name="item">생성된 인스턴스. <see langword="null"/>일 수 있습니다.</param>
/// <returns>종료 코드. 앱을 계속 실행하려면 0을 반환합니다.</returns>
public delegate int ParseResultToItemFactory<TItem>(ParseResult parseResult, out TItem? item);