<template>
	<el-skeleton :loading="datas.loading" style="height:100%;" animated>
		<template #template>
			<div class="radius-box marb-30 pad-20">
				<el-skeleton-item variant="h3" style="margin-top:1rem;width:60%;" />
				<el-skeleton-item variant="h3" style="margin:1rem 0;width:100%;" />
				<el-skeleton-item variant="p" style="margin-bottom:1rem;width:80%;" />
			</div>
			<div class="radius-box marb-30 pad-20">
				<el-skeleton-item variant="h3" style="margin-top:1rem;width:60%;" />
				<el-skeleton-item variant="p" style="margin-bottom:1rem;width:80%;" />
			</div>
			<div class="radius-box marb-30 pad-20">
				<el-skeleton-item variant="p" style="margin-bottom:1rem;width:80%;" />
			</div>
		</template>
		<div class="radius-box mart-20 pad-30">
			<div class="title">交易编号：{{datas.model.tradeNo}}</div>
			<div>
				<el-result v-if="datas.model.paymentType>0"
					icon="success"
					title="下单成功"
					sub-title="已经下单成功,请等待商家处理.">
					<template #extra>
						<el-button type="success" @click="common.linkUrl('/account/recharge')" v-if="datas.model.tradeType==1">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/subscription')" v-else-if="datas.model.tradeType==2">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/order/list')" v-else>查看订单</el-button>
						<el-button type="primary" @click="common.linkUrl('/account')">会员中心</el-button>
					</template>
				</el-result>
				<el-result v-else-if="datas.model.status===1"
					icon="warning"
					title="支付未完成"
					sub-title="支付尚未处理成功,请稍候查看.">
					<template #extra>
						<el-button type="success" @click="common.linkUrl('/account/recharge')" v-if="datas.model.tradeType==1">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/subscription')" v-else-if="datas.model.tradeType==2">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/order/list')" v-else>查看订单</el-button>
						<el-button type="primary" @click="common.linkUrl('/account')">会员中心</el-button>
					</template>
				</el-result>
				<el-result v-else-if="datas.model.status===2"
					icon="success"
					title="支付成功"
					sub-title="已经付款成功,可以点击查看详情.">
					<template #extra>
						<el-button type="success" @click="common.linkUrl('/account/recharge')" v-if="datas.model.tradeType==1">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/subscription')" v-else-if="datas.model.tradeType==2">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/order/list')" v-else>查看订单</el-button>
						<el-button type="primary" @click="common.linkUrl('/account')">会员中心</el-button>
					</template>
				</el-result>
				<el-result v-else-if="datas.model.status===3"
					icon="error"
					title="交易取消"
					sub-title="该交易已经取消,请重新发起.">
					<template #extra>
						<el-button type="success" @click="common.linkUrl('/account/recharge')" v-if="datas.model.tradeType==1">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/subscription')" v-else-if="datas.model.tradeType==2">查看订单</el-button>
						<el-button type="success" @click="common.linkUrl('/account/order/list')" v-else>查看订单</el-button>
						<el-button type="primary" @click="common.linkUrl('/account')">会员中心</el-button>
					</template>
				</el-result>
			</div>
		</div>
	</el-skeleton>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `支付结果 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	
	//获取路由信息
	const route = useRoute()
	//声明变量
	const datas = reactive({
		loading: true,
		model: {}, //交易详情
	})
	
	//初始化数据
	const initData = async() => {
		const tradeNo = route.query.no
		if(!tradeNo) {
			ElMessageBox.alert('收款交易号有误，请检查重试', '错误提示', {
				confirmButtonText: '确定',
				callback: action => {
					common.back(-1)
				}
			})
			return
		}
		//查询交易收款单
		await useHttp({
			url: `/account/order/payment/${tradeNo}`,
			beforeSend() {
				datas.loading = true
			},
			success: async(res) => {
				datas.model = res.data
			},
			error(err) {
				console.log(err)
			},
			complete() {
				datas.loading = false
			}
		}).catch(err => {})
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>