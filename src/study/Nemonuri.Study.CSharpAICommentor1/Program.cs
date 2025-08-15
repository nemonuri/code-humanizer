
using Nemonuri.Study.CSharpAICommentor1;

// 

StatusForTest.AssertConfigured();

// 

StatusForTest.AssertOriginalSyntaxTreeIsValid();

//

StatusForTest.AssertRewritedSyntaxTreeIsValid();

//

StatusForTest.AssertCommentTriviaAddedTreeIsValid();

//

StatusForTest.AssertAICommentedCodeIsValid();