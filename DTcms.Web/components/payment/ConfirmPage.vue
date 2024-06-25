<template>
	<div class="payment-cart-box">
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
				<div class="ntitle">订单信息</div>
				<div class="row-box">
					<div class="item-box">
						<div class="title">交易编号</div>
						<div class="content">{{datas.model.tradeNo}}</div>
					</div>
					<div class="item-box">
						<div class="title">交易类型</div>
						<div class="content">
							<span v-if="datas.model.tradeType===1">会员充值</span>
							<span v-else-if="datas.model.tradeType===2">会员订购</span>
							<span v-else>商品订单</span>
							<span v-if="datas.model.tradeMode===1">（预售定金）</span>
							<span v-if="datas.model.tradeMode===2">（预售尾款）</span>
							<span v-else>（全额付款）</span>
						</div>
					</div>
					<div class="item-box">
						<div class="title">支付金额</div>
						<div class="content">{{datas.model.paymentAmount}}元</div>
					</div>
				</div>
			</div>
			<div class="bar-title mart-20">
				<div class="title">支付方式</div>
			</div>
			<!--支付方式-->
			<div class="payment-box">
				<div v-for="(item,index) in datas.paymentList"
					:key="index"
					:class="{active:datas.paymentModel.id===item.id}"
					@click="handlePaymentChange(item)"
					class="list-box radius-box">
					<span v-if="item.paymentType===1" class="icon iconfont icon-balance"></span>
					<span v-else-if="item.paymentType===2" class="icon iconfont icon-weixin"></span>
					<span v-else-if="item.paymentType===3" class="icon iconfont icon-alipay"></span>
					<span v-else class="icon iconfont icon-trade-full"></span>
					<div class="txt-box">
						<span class="name">{{item.title}}</span>
						<span class="text">{{item.remark}}</span>
					</div>
				</div>
			</div>
			<!--底部-->
			<div class="foot-box radius-box mart-20">
				<div class="left" v-if="datas.model.endTime">
					<span class="text">支付剩余时间</span>
					<div class="text">
						<el-countdown format="DD[天]HH:mm:ss" :value="common.convertDateTime(datas.model.endTime)"></el-countdown>
					</div>
				</div>
				<div class="right">
					<el-button type="danger" :icon="ElIconCreditCard" @click="handleConfirm">确认付款</el-button>
				</div>
			</div>
		</el-skeleton>
	</div>
	<el-dialog v-model="datas.codeVisible" title="扫码支付">
		<div class="native-box">
			<div class="img-box">
				<img :src="`data:text/css;base64,${codeData}`">
			</div>
			<span class="text">打开微信扫一扫完成支付</span>
		</div>
	</el-dialog>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `支付中心 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//获取路由信息
	const route = useRoute()
	//声明变量
	const datas = reactive({
		loading: true,
		codeVisible: false, //二维码显示隐藏
		codeData: null, //BASE64图片
		model: {}, //交易详情
		paymentList: [], //支付方式列表
		paymentModel: {}, //当前支付方式
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
				//判断是否已支付,如果是已支付则直接跳转到成功页面
				if (datas.model.paymentType > 0 || datas.model.status > 1) {
					//跳转链接
					await navigateTo({ path: '/payment/result', query: {
						no: datas.model.tradeNo,
					}})
				}
			},
			error(err) {
				console.log(err)
			},
			complete() {
				datas.loading = false
			}
		}).catch(err => {})
		
		//加载支付方式列表
		let sendUrl = `/client/payment/view/10?siteId=${siteConfig.id}`
		if (datas.model.tradeType === 1) {
			sendUrl += `&types=pc,native`
		} else if(datas.model.tradeType === 2){
			sendUrl += `&types=balance,pc,native`
		} else {
			sendUrl += `&types=balance,cash,pc,native`
		}
		await useHttp({
			url: sendUrl,
			success(res) {
				datas.paymentList = res.data
			}
		}).catch(err => {})
		
		//选中订单支付方式
		const obj = datas.paymentList.find(item => item.id === datas.model.paymentId)
		if (obj) {
			datas.paymentModel = obj
		}
	}
	//选择支付方式
	const handlePaymentChange = async(item) => {
		if (datas.model.paymentId === item.id) {
			return
		}
		//赋值给实体
		datas.model.paymentId = item.id
		datas.paymentModel = item
	}
	//确认支付
	const handleConfirm = async() => {
		if (!datas.paymentModel.type) {
			ElMessage.warning('请选择付款方式')
			return
		}
		//提交修改支付方式
		await useHttp({
			method: "put",
			url: `/account/order/payment/edit`,
			loading: true,
			data: {
				id: datas.model.id,
				paymentId: datas.paymentModel.id
			},
			success(res) {
				datas.model = res.data
			}
		})
		//判断支付方式选择不同的接口
		if (datas.paymentModel.type == "cash") {
			await cashPay()
		}
		if (datas.paymentModel.type == "balance") {
			await balancePay()
		}
		if (datas.paymentModel.type == "pc") {
			await pcPay()
		}
		if (datas.paymentModel.type == "native") {
			await nativePay()
		}
	}
	//线下支付
	const cashPay = async() => {
		//跳转链接
		await navigateTo({ path: '/payment/result', query: {
			no: datas.model.tradeNo,
		}})
	}
	//余额支付
	const balancePay = async() => {
		let pay = datas.paymentModel
		//调用下单接口
		await useHttp({
			method: "post",
			url: `${pay.payUrl}`,
			loading: true,
			data: {
				outTradeNo: datas.model.tradeNo
			},
			success: async(res) => {
				//跳转链接
				await navigateTo({ path: '/payment/result', query: {
					no: datas.model.tradeNo,
				}})
			}
		})
	}
	//电脑支付
	const pcPay = async() => {
		let pay = datas.paymentModel
		//获取当前页面
		let href = window.location.protocol + "//" + window.location.host
		let uri = encodeURIComponent(href + `/payment/result?no=${datas.model.tradeNo}`)
		//调用统一下单接口
		await useHttp({
			method: "post",
			url: `${pay.payUrl}`,
			loading: true,
			data: {
				outTradeNo: datas.model.tradeNo,
				description: "商品订单",
				returnUri: uri
			},
			success: async(res) => {
				await navigateTo(res.data.url)
			}
		})
	}
	//扫码支付
	const nativePay = async() => {
		let pay = datas.paymentModel
		//调用统一下单接口
		await useHttp({
			method: "post",
			url: `${pay.payUrl}`,
			loading: true,
			data: {
				outTradeNo: datas.model.tradeNo,
				description: "商品订单"
			},
			success(res) {
				datas.codeVisible = true
				datas.codeData = res.data.codeData
				//5秒后调用监听事件
				setTimeout(() => {
					handleNative(datas.model.tradeNo)
				}, 5000)
			}
		})
	}
	//监听订单是否支付
	const handleNative = async(no) => {
		let status = false
		await useHttp({
			url: `/account/order/payment/${no}`,
			success(res) {
				if (res.data.status > 1) {
					status = true
				}
			}
		})
		//判断是否已支付,如果是已支付则直接跳转到成功页面
		if (status) {
			//跳转链接
			await navigateTo({ path: '/payment/result', query: { no: no }})
		} else if (datas.codeVisible) {
			//5秒后调用监听事件
			setTimeout(() => {
				handleNative(no)
			}, 5000)
		}
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>

<style lang="scss">
	.payment-cart-box {
		.payment-box {
			.list-box {
				display: flex;
				align-items: center;
				position: relative;
				margin-top: 1.25rem;
				padding: 1.25rem;
				cursor: pointer;
				border: 1px solid rgba(0, 0, 0, 0.03);
				&:hover {
					border: 1px solid #d4d4d4;
				}
				&.active {
					border: 1px solid #d4d4d4;
					&:before {
						content: "\e618";
						font-family: "iconfont";
						display: block;
						position: absolute;
						right: 1px;
						bottom: 6px;
						color: #e93323;
						font-size: 2rem;
					}
				}
				.icon {
					color: #fe9c01;
					font-size: 2.5rem;
					margin-right: 1rem;
					&.icon-weixin{
						color: #09bb07;
					}
					&.icon-alipay{
						color: #00aaea;
					}
				}
				.txt-box {
					display: flex;
					flex-direction: column;
					.name {
						color: #4e4e4e;
						font-size: 1rem;
						line-height: 1em;
					}
					.text {
						margin-top: 0.5rem;
						color: #969696;
						font-size: 0.875rem;
						line-height: 1.5em;
					}
				}
			}
		}
		.foot-box {
			display: flex;
			justify-content: space-between;
			align-items: center;
			flex-flow: row wrap;
			padding: 1.25rem 1.5rem;
			.left {
				display: flex;
				align-items: center;
				margin-left: -1rem;
				color: rgba(0, 0, 0, 0.6);
				.text {
					margin-left: 1rem;
					font-size: 1rem;
					.el-statistic__content {
						color: #e93323;
						font-size: 1rem;
					}
				}
			}
			.right {
				flex-grow: 1;
				display: flex;
				align-items: center;
				justify-content: flex-end;
				.text {
					margin-right: 1rem;
					font-size: 1rem;
					b {
						color: #e93323;
						font-size: 1rem;
					}
				}
			}
		}
	}
	.native-box {
		display: flex;
	    flex-direction: column;
	    align-items: center;
		.img-box {
			position: relative;
			width: 298px;
			height: 298px;
			img,el-image{
				width: 100%;
				height: 100%;
			}
		}
		.text {
			margin-top: 20px;
			color: #999;
			&:before {
				margin-right: 3px;
			}
		}
	}
</style>