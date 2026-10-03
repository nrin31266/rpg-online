using System.Collections;
using HuyenLo.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HuyenLo.Tests
{
    public sealed class ContinuousRouteTests
    {
        [UnityTest, Timeout(180000)]
        public IEnumerator Q1ToQ6UsesRealPhysicsAndNoQuestOrPositionInjection() {
            var go=new GameObject("VS1 route under test");var host=go.AddComponent<SliceHost>();
            try {
            var driver=go.AddComponent<SliceRouteProbe>();yield return driver.Run(host);
            Assert.That(driver.Failure,Is.Null);Assert.That(driver.Finished,Is.True);Assert.That(host.Session.Complete,Is.True);
            Assert.That(host.Session.Receipts.Contains("Q2.dropped"),Is.True);
            } finally {Object.Destroy(go);}
            yield return null;
        }
    }
}
