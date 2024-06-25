<template>
	<!--内容详情-->
	<div class="detail-box radius-box">
		<div class="h3" :style="`text-align:${props.align};`">{{datas.model.title}}</div>
		<div class="meta" v-if="props.isMeta">
			<span class="text">来源：{{datas.model.source}}</span>
			<span class="text click" @click="updateLike">
				<i class="iconfont icon-good"></i>
				{{datas.model.likeCount}}
			</span>
			<span class="text">
				<i class="iconfont icon-msg"></i>
				{{datas.model.commentCount}} 评论
			</span>
			<span class="text">
				<i class="iconfont icon-date"></i>
				{{datas.model.addTime}}
			</span>
		</div>
		<!--播放器-->
		<div class="payer-box" v-show="datas.player">
			<div ref="videoRef"></div>
		</div>
		<!--相册列表-->
		<div class="album-list" v-if="datas.model.articleAlbums.length>0">
			<div class="list-box" v-for="(item,index) in datas.model.articleAlbums" :key="index">
				<div class="img-box">
					<el-image :src="common.loadFile(item.thumbPath)" :preview-src-list="replaceFilePath(datas.model.articleAlbums,'originalPath')" :initial-index="index" fit="cover" lazy></el-image>
				</div>
				<div class="txt-box" v-if="item.remark">
					<span class="text">{{item.remark}}</span>
				</div>
			</div>
		</div>
		<!--附件列表-->
		<div class="attach-list" v-if="datas.model.articleAttachs.length>0">
			<div class="list-box" v-for="(item,index) in datas.model.articleAttachs" :key="index">
				<div class="left-box">
					<span class="icon iconfont icon-attach"></span>
					<div class="txt-box">
						<span class="title">{{item.fileName}}</span>
						<div class="info">
							<span>文件类型：{{item.fileExt}}</span>
							<span>文件大小：{{item.size}}</span>
							<span>下载次数：{{item.downCount}} 次</span>
						</div>
					</div>
				</div>
				<div class="btn-box">
					<el-button :loading="datas.btnLoading"
						:disabled="datas.btnLoading"
						:icon="ElIconDownload"
						@click="downFile(item.id, item.fileName)">下载</el-button>
				</div>
			</div>
		</div>
		<div class="content" v-html="common.replaceHtml(datas.model.content)"></div>
	</div>
	
	<slot :articleId="datas.model.id" :isComment="datas.model.isComment>0"></slot>
	
</template>

<script setup>
	import Player from 'xgplayer'
	import 'xgplayer/dist/index.min.css'
	import 'xgplayer/es/plugins/danmu/index.css'
	
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//Ref定义
	const videoRef = ref(null)
	//接收props传值
	const props = defineProps({
		//标题对齐
		align: {
			type: String,
			default: () => {
				return 'left'
			}
		},
		//显示眉头
		isMeta: {
			type: Boolean,
			default: () => {
				return true
			}
		},
	})
	//声明变量
	const datas = reactive({
		articleKey: null,
		btnLoading: false,
		model: {},
		player: null,
	})
	
	//获取路由信息
	const route = useRoute()
	if(route.params.id) {
		datas.articleKey = route.params.id
		//服务端获取当前信息
		const {data: detailRes, error} = await useAsyncData('detail', () => useHttp({url: `/client/article/show/${datas.articleKey}`}))
		datas.model = detailRes.value?.data
	}
	
	//如果有错误则抛出异常
	if(!datas.model.id) {
		throw createError({
			statusCode: error.value.statusCode,
			message: error.value.message,
			fatal: true
		})
	}
	//页面SEO设置
	useSeoMeta({
		title: `${datas.model.title}`,
		ogTitle: datas.model.seoKeyword ?? siteConfig.seoKeyword,
		description: datas.model.seoDescription ?? siteConfig.seoDescription,
		ogDescription: datas.model.seoDescription ?? siteConfig.seoDescription,
	})
	
	//替换相册原图绝对路径
	const replaceFilePath = (list, propName) => {
		if(!list) return
		let values = []
		for (let i = 0; i < list.length; i++) {
			if (list[i].hasOwnProperty(propName)) {
				values.push(common.loadFile(list[i][propName]))
			}
		}
		return values
	}
	//下载附件
	const downFile = (id, fileName) => {
		useHttp({
			method: "get",
			url: `/client/article/download/${id}`,
			beforeSend() {
				datas.btnLoading = true
			},
			success(res) {
				if (res.contentType && res.contentType.includes('application/octet-stream')) {
					const blob = new Blob([res.data], { type: res.contentType }) //生成blob文件
					const url = URL.createObjectURL(blob) //生成URL
					let a = document.createElement("a")
					a.href = url
					a.download = fileName
					a.click()
					a.remove()
					URL.revokeObjectURL(a.href)
				}
			},
			complete() {
				datas.btnLoading = false
			}
		})
	}
	//文章点赞
	const updateLike = () => {
		useHttp({
			url: `/account/article/like/${datas.model.id}`,
			method: "put",
			success(res) {
				datas.model.likeCount = res.data
			}
		})
	}
	//初始化播放器
	const initPlayer = () => {
		if (!datas.model.videoUrl) {
			return null
		}
		let player = new Player({
			el: videoRef.value,
			poster: common.loadFile(datas.model.imgUrl),
			url: common.loadFile(datas.model.videoUrl),
			height: '100%',
			width: '100%',
			autoplay: true
		})
		return player
	}
	
	//页面加载完成
	onMounted(() => {
		//初始化播放器
		datas.player = initPlayer()
	})
</script>